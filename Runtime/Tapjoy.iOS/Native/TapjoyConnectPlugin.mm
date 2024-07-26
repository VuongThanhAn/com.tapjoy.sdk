#import "TapjoyConnectPlugin.h"
#import "TapjoyPlacementPlugin.h"
#import "TapjoyOfferwallDiscoverPlugin.h"

UIViewController *UnityGetGLViewController();

static TapjoyConnectPlugin *_sharedInstance = nil; //To make TapjoyConnect Singleton
static TJPrivacyPolicy *privacyPolicyInstance = nil;
static TapjoyOfferwallDiscoverPlugin *offerwallDiscoverPlugin = nil;

@implementation TapjoyConnectPlugin


NSString *const UNITY_GAME_OBJECT_NAME = @"TapjoyUnity";
NSString *const CONNECT_FLAG_KEY = @"connectFlags";
NSCharacterSet *URLFullCharacterSet;


+ (void)initialize
{
	if (self == [TapjoyConnectPlugin class])
	{
		_sharedInstance = [[self alloc] init];
		URLFullCharacterSet = [[NSCharacterSet characterSetWithCharactersInString:@" \"#%/:<>?@[\\],^`{|}"] invertedSet];
	}
}

+ (TapjoyConnectPlugin*)sharedTapjoyConnectPlugin
{
	return _sharedInstance;
}

- (id)init
{
	self = [super init];

    if (self)
    {
        tapPoints = 0;
        cSharpDictionaryRefs_ = [[NSMutableDictionary alloc] init];
        actionRequestDict_ = [[NSMutableDictionary alloc] init];
        placementDict_ = [[NSMutableDictionary alloc] init];
        placementDelegateDict_ = [[NSMutableDictionary alloc] init];
        offerwallDiscoverPlugin = [[TapjoyOfferwallDiscoverPlugin alloc] init];

        // Connect Notifications
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(onConnectSuccess:)
                                                     name:TJC_CONNECT_SUCCESS
                                                   object:nil];
        [[NSNotificationCenter defaultCenter] addObserver:self
        										 selector:@selector(onConnectFailure:)
        										     name:TJC_CONNECT_FAILED
        										   object:nil];
		[[NSNotificationCenter defaultCenter] addObserver:self
        										 selector:@selector(onConnectWarning:)
        										     name:TJC_CONNECT_WARNING
        										   object:nil];

        // Currency
        [[NSNotificationCenter defaultCenter] addObserver:self
                                                 selector:@selector(earnedCurrency:)
                                                     name:TJC_CURRENCY_EARNED_NOTIFICATION
                                                   object:nil];
    }

	return self;
}

#pragma mark Tapjoy Connect Notifications
- (void)onConnectSuccess:(NSNotification*)notifyObj
{
	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeConnectCallback", "OnConnectSuccess");
}


- (void)onConnectFailure:(NSNotification*)notifyObj
{
    NSError *error = notifyObj.userInfo[@"error"];
    NSMutableString *errorMessage = @"OnConnectFailure".mutableCopy;

    if (error != nil) {
        NSError *underlyingError = error.userInfo[NSUnderlyingErrorKey];
        if (underlyingError != nil) {
            [errorMessage appendFormat:@",%ld,%@", underlyingError.code, underlyingError.localizedDescription];
        } else {
            [errorMessage appendFormat:@",%ld,%@", error.code, error.localizedDescription];
        }
    }
	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeConnectCallback", [errorMessage UTF8String]);
}

- (void)onConnectWarning:(NSNotification*)notifyObj
{
	NSError *error = notifyObj.userInfo[@"error"];
	NSMutableString *errorMessage = @"OnConnectWarning".mutableCopy;

	if (error != nil) {
		NSError *underlyingError = error.userInfo[NSUnderlyingErrorKey];
 		if (underlyingError != nil) {
            [errorMessage appendFormat:@",%ld,%@", underlyingError.code, underlyingError.localizedDescription];
        } else {
            [errorMessage appendFormat:@",%ld,%@", error.code, error.localizedDescription];
        }
	}
	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeConnectCallback", [errorMessage UTF8String]);
}

#pragma mark Tapjoy Currency Methods
- (void)createCurrencyCallback:(NSString *)callbackName withParameters:(NSDictionary *)parameters andError:(NSError *)error
{
	NSString *currencyName = parameters[@"currencyName"];
	int balance = [parameters[@"amount"] intValue];

	if (error == nil)
	{
		NSMutableString *parameters = [[NSMutableString alloc] init];
		[parameters appendString:callbackName];
		[parameters appendString:@","];
		[parameters appendString: currencyName];
		[parameters appendString:@","];
		[parameters appendString: [NSString stringWithFormat:@"%i", balance]];

		UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeCurrencyCallback", [parameters UTF8String]);
	}
	else
	{
		NSError *errorValue = [NSError errorWithDomain:error.domain code:error.code userInfo:nil];

		NSMutableString *parameters = [[NSMutableString alloc] init];
		[parameters appendString:[NSString stringWithFormat:@"%@Failure", callbackName]];
		[parameters appendString:@","];
		[parameters appendString: [errorValue localizedDescription]];

		UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeCurrencyCallback", [parameters UTF8String]);

	}
}

- (void)earnedCurrency:(NSNotification*)notifyObj
{
	NSNumber *tapPointsEarned = notifyObj.object;
	earnedCurrencyAmount = [tapPointsEarned intValue];

	NSString *currencyName = [[NSUserDefaults standardUserDefaults] stringForKey:@"TJC_CURRENCY_KEY_NAME"];
	if (!currencyName) {
		currencyName = @"";
	}

	NSMutableString *parameters = [[NSMutableString alloc] init];
	[parameters appendString:@"OnEarnedCurrency,"];
	[parameters appendString:currencyName];
	[parameters appendString:@","];
	[parameters appendString: [NSString stringWithFormat:@"%i", earnedCurrencyAmount]];

	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeCurrencyCallback", [parameters UTF8String]);
}

#pragma mark Tapjoy Placement Methods

- (void)createPlacement:(NSString *)guid withName:(NSString *)name
{
	TapjoyPlacementPlugin *placementDelegate = [TapjoyPlacementPlugin createPlacement:guid withName:name];

	TJPlacement *placement;
	placement = [TJPlacement placementWithName:name delegate:placementDelegate];

	if (placement == nil) {
		return;
	}

	[placementDict_ setObject:placement forKey:guid];
	[placementDelegateDict_ setObject:placementDelegate forKey:guid];
}

- (void)requestPlacementContentWithGuid:(NSString *)guid
{
 	TJPlacement *placement = [placementDict_ objectForKey:guid];
 	if (placement != nil) {
        [placement setTopViewControllerClassName:[self getUnityViewControllerName]];
 		[placement setPresentationViewController:UnityGetGLViewController()];
		[placement requestContent];
	}
}

- (void)showPlacementContentWithGuid:(NSString *)guid
{
	TJPlacement *placement = [placementDict_ objectForKey:guid];
	if (placement != nil) {
		[placement showContentWithViewController:UnityGetGLViewController()];
	}
}

- (NSString*)getUnityViewControllerName
{
    NSString * presentationClassName = @"";
    NSString * presentingClassName = @"";
    
    presentationClassName = NSStringFromClass(UnityGetGLViewController().class);
    
    if ([presentationClassName isEqualToString:@"UnityDefaultViewController"]) {
        presentingClassName = @"TapjoyUnityDefaultViewController";
    } else if ([presentationClassName isEqualToString:@"UnityPortraitOnlyViewController"]) {
        presentingClassName = @"TapjoyUnityPortraitOnlyViewController";
    } else if ([presentationClassName isEqualToString:@"UnityPortraitUpsideDownOnlyViewController"]) {
        presentingClassName = @"TapjoyUnityPortraitUpsideDownOnlyViewController";
    } else if ([presentationClassName isEqualToString:@"UnityLandscapeLeftOnlyViewController"]) {
        presentingClassName = @"TapjoyUnityLandscapeLeftOnlyViewController";
    } else if ([presentationClassName isEqualToString:@"UnityLandscapeRightOnlyViewController"]) {
        presentingClassName = @"TapjoyUnityLandscapeRightOnlyViewController";
    }
    
    return presentingClassName;
}

- (bool)isPlacementContentReady:(NSString *) guid
{
	TJPlacement *placement = [placementDict_ objectForKey:guid];
	if (placement != nil) {
		return [placement isContentReady];
	}
	return false;
}

- (bool)isPlacementContentAvailable:(NSString *)guid
{
	TJPlacement *placement = [placementDict_ objectForKey:guid];
	if (placement != nil) {
		return [placement isContentAvailable];
	}
	return false;
}

- (void)setPlacementBalance:(NSInteger)amount forCurrencyId:(NSString *)currencyId guid:(NSString *)guid
{
    TJPlacement *placement = [placementDict_ objectForKey:guid];
    if (placement != nil) {
        [placement setBalance:amount forCurrencyId:currencyId withCompletion:^(NSError *error) {
            if (error != nil) {
                NSMutableString *parameters = [[NSMutableString alloc] init];
                [parameters appendString:@"OnSetCurrencyBalanceFailure,"];
                [parameters appendString:guid];
                [parameters appendString:@","];
                [parameters appendString:placement.placementName];
                [parameters appendString:@","];
                [parameters appendString:[NSString stringWithFormat:@"%ld", error.code]];
                [parameters appendString:@","];
                [parameters appendString:error.localizedDescription];
                UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [parameters UTF8String]);
            }else{
                NSMutableString *parameters = [[NSMutableString alloc] init];
                [parameters appendString:@"OnSetCurrencyBalanceSuccess,"];
                [parameters appendString:guid];
                [parameters appendString:@","];
                [parameters appendString:placement.placementName];
                UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [parameters UTF8String]);
            }
        }];
    }
}

- (NSInteger)getPlacementBalanceForCurrencyId:(NSString *)currencyId guid:(NSString *)guid
{
    TJPlacement *placement = [placementDict_ objectForKey:guid];
    if (placement != nil) {
        return [placement getBalanceForCurrencyId:currencyId];
    }
    return -1;
}

- (void)setRequiredAmount:(NSInteger)amount forCurrencyId:(NSString *)currencyId guid:(NSString *)guid
{
    TJPlacement *placement = [placementDict_ objectForKey:guid];
    if (placement != nil) {
        [placement setRequiredAmount:amount forCurrencyId:currencyId withCompletion:^(NSError *error) {
            if (error != nil) {
                NSMutableString *parameters = [[NSMutableString alloc] init];
                [parameters appendString:@"OnSetRequiredAmountFailure,"];
                [parameters appendString:guid];
                [parameters appendString:@","];
                [parameters appendString:placement.placementName];
                [parameters appendString:@","];
                [parameters appendString:[NSString stringWithFormat:@"%ld", error.code]];
                [parameters appendString:@","];
                [parameters appendString:error.localizedDescription];
                UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [parameters UTF8String]);
            }else{
                NSMutableString *parameters = [[NSMutableString alloc] init];
                [parameters appendString:@"OnSetRequiredAmountSuccess,"];
                [parameters appendString:guid];
                [parameters appendString:@","];
                [parameters appendString:placement.placementName];
                UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [parameters UTF8String]);
            }
        }];
    }
}

- (NSInteger)getRequiredAmountForCurrencyId:(NSString *)currencyId guid:(NSString *)guid
{
    TJPlacement *placement = [placementDict_ objectForKey:guid];
    if (placement != nil) {
        return [placement getRequiredAmountForCurrencyId:currencyId];
    }
    return -1;
}

- (void)actionRequestCompleted:(NSString *) requestId
{
	TJActionRequest *actionRequest = [actionRequestDict_ objectForKey:requestId];
	if (actionRequest)
	{
		NSLog(@"Sending TJPlacementRequest completed");
		[actionRequest completed];
	}
}

- (void)actionRequestCancelled:(NSString *) requestId
{
	TJActionRequest *actionRequest = [actionRequestDict_ objectForKey:requestId];
	if(actionRequest)
	{
		NSLog(@"Sending TJPlacementRequest cancelled");
		[actionRequest cancelled];
	}
}

- (void)removePlacement:(NSString *)guid
{
	[placementDict_ removeObjectForKey:guid];
	[placementDelegateDict_ removeObjectForKey:guid];
}

- (void)removeActionRequest:(NSString *) requestId
{
	[actionRequestDict_ removeObjectForKey:requestId];
}

#pragma mark - Tapjoy Static Placement Delegate Methods

- (void)requestDidSucceed:(NSString *)guid withName:(NSString *)placementName withContent:(BOOL)contentIsAvailable
{
	placementName = [placementName stringByAddingPercentEncodingWithAllowedCharacters:URLFullCharacterSet];
	NSMutableString *parameters = [[NSMutableString alloc] init];
	[parameters appendString:@"OnPlacementRequestSuccess,"];
	[parameters appendString:guid];
	[parameters appendString:@","];
	[parameters appendString:placementName];
	[parameters appendString:@","];
	[parameters appendString:[NSString stringWithFormat:@"%s", contentIsAvailable ? "true" : "false"]];

	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [parameters UTF8String]);
}

- (void)requestDidFail:(NSString *)guid withName:(NSString *)placementName error:(NSError*)error
{
	placementName = [placementName stringByAddingPercentEncodingWithAllowedCharacters:URLFullCharacterSet];
	NSMutableString *parameters = [[NSMutableString alloc] init];
	[parameters appendString:@"OnPlacementRequestFailure,"];
	[parameters appendString:guid];
	[parameters appendString:@","];
	[parameters appendString:placementName];
	[parameters appendString:@","];

	NSString *errorString = [error localizedDescription];
	if (errorString == nil) {
		errorString = @"";
	}

	[parameters appendString: errorString];

	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [parameters UTF8String]);
}

- (void)contentIsReady:(NSString *)guid withName:(NSString *)placementName
{
	placementName = [placementName stringByAddingPercentEncodingWithAllowedCharacters:URLFullCharacterSet];

	NSMutableString *parameters = [[NSMutableString alloc] init];
	[parameters appendString:@"OnPlacementContentReady,"];
	[parameters appendString:guid];
	[parameters appendString:@","];
	[parameters appendString:placementName];
	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [parameters UTF8String]);
}

- (void)contentDidAppear:(NSString *)guid withName:(NSString *)placementName
{
	placementName = [placementName stringByAddingPercentEncodingWithAllowedCharacters:URLFullCharacterSet];

	NSMutableString *parameters = [[NSMutableString alloc] init];
	[parameters appendString:@"OnPlacementContentShow,"];
	[parameters appendString:guid];
	[parameters appendString:@","];
	[parameters appendString:placementName];

	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [parameters UTF8String]);
}

- (void)contentDidDisappear:(NSString *)guid withName:(NSString *)placementName
{
	placementName = [placementName stringByAddingPercentEncodingWithAllowedCharacters:URLFullCharacterSet];

	NSMutableString *parameters = [[NSMutableString alloc] init];
	[parameters appendString:@"OnPlacementContentDismiss,"];
	[parameters appendString:guid];
	[parameters appendString:@","];
	[parameters appendString:placementName];

	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [parameters UTF8String]);
}

- (void)didClick:(NSString *)guid withName:(NSString *)placementName
{
	placementName = [placementName stringByAddingPercentEncodingWithAllowedCharacters:URLFullCharacterSet];

	NSMutableString *parameters = [[NSMutableString alloc] init];
	[parameters appendString:@"OnPlacementClick,"];
	[parameters appendString:guid];
	[parameters appendString:@","];
	[parameters appendString:placementName];
	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [parameters UTF8String]);
}

- (void)placement:(NSString *)guid withName:(NSString *)placementName didRequestPurchase:(TJActionRequest*)request productId:(NSString*)productId
{
	[actionRequestDict_ setObject:request forKey:guid];

	placementName = [placementName stringByAddingPercentEncodingWithAllowedCharacters:URLFullCharacterSet];

	//TODO: use json encoding
	NSString *message = [NSString stringWithFormat: @"OnPurchaseRequest,%@,%@,%@,%@,%@", guid, placementName, request.requestId, request.token, productId];
	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [message UTF8String]);
}

- (void)placement:(NSString *)guid withName:(NSString *)placementName didRequestReward:(TJActionRequest*)request itemId:(NSString*)itemId quantity:(int)quantity
{
	[actionRequestDict_ setObject:request forKey:guid];

	placementName = [placementName stringByAddingPercentEncodingWithAllowedCharacters:URLFullCharacterSet];

	//TODO: use json encoding
	NSString *message = [NSString stringWithFormat: @"OnRewardRequest,%@,%@,%@,%@,%@,%i", guid, placementName, request.requestId, request.token, itemId, quantity];

	UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativePlacementCallback", [message UTF8String]);
}


- (void)setEntryPoint:(NSString *)guid
                         value:(NSString *)value {
  TJPlacement *placement = [placementDict_ objectForKey:guid];
  if (placement != nil) {
      const char *cStringValue = [value UTF8String];
      placement.entryPoint = TJEntryPointFromString(cStringValue);
  }
}

#pragma mark Offerwall Discover

// Request
- (void)requestOfferwallDiscover:(NSString*)placementName height:(CGFloat)height {
    [offerwallDiscoverPlugin request:placementName height:(CGFloat)height];
}

- (void)requestOfferwallDiscover:(NSString*)placementName left:(CGFloat)left top:(CGFloat)top width:(CGFloat)width height:(CGFloat)height {
    [offerwallDiscoverPlugin request:placementName left:(CGFloat)left top:(CGFloat)top width:(CGFloat)width height:(CGFloat)height];
}

// Show
- (void)showOfferwallDiscover {
    [offerwallDiscoverPlugin show:UnityGetGLView()];
}

// Destroy
- (void)destroyOfferwallDiscover {
    [offerwallDiscoverPlugin destroy];
}

- (void)offerwallDiscoverRequestDidSucceed {
    NSMutableString *parameters = [[NSMutableString alloc] init];
    [parameters appendString:@"OnOfferwallDiscoverRequestSuccess"];

    UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeOfferwallDiscoverCallback", [parameters UTF8String]);
}

- (void)offerwallDiscoverRequestDidFail:(NSError *)error {
    NSMutableString *parameters = [[NSMutableString alloc] init];
    [parameters appendString:@"OnOfferwallDiscoverRequestFailure"];
    [parameters appendString:@","];

    [parameters appendString:[NSString stringWithFormat:@"%li", [error code]]];
    [parameters appendString:@","];

    NSString *errorString = [error localizedDescription];
    if (errorString == nil) {
        errorString = @"";
    }

    [parameters appendString: errorString];

    UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeOfferwallDiscoverCallback", [parameters UTF8String]);
}

- (void)offerwallDiscoverContentReady {
    NSMutableString *parameters = [[NSMutableString alloc] init];
    [parameters appendString:@"OnOfferwallDiscoverContentReady"];

    UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeOfferwallDiscoverCallback", [parameters UTF8String]);
}

- (void)offerwallDiscoverContentError:(NSError *)error {
    NSMutableString *parameters = [[NSMutableString alloc] init];
    [parameters appendString:@"OnOfferwallDiscoverContentError"];
    [parameters appendString:@","];

    [parameters appendString:[NSString stringWithFormat:@"%li", [error code]]];
    [parameters appendString:@","];

    NSString *errorString = [error localizedDescription];
    if (errorString == nil) {
        errorString = @"";
    }

    [parameters appendString: errorString];

    UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeOfferwallDiscoverCallback", [parameters UTF8String]);
}

#pragma mark Bridge methods between C# and Objective-C

// Sets a value to a key in specific dictionary
- (void)setKey:(NSString*)key ToValue:(NSString*)value InDictionary:(NSString*)dictionaryToAddTo
  {
    // Get dictionary to add key and value to, creates one if doesn't exist
    NSMutableDictionary* currentDictionary = [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] getReferenceDictionary:dictionaryToAddTo];

    [currentDictionary setObject:value forKey:key];
  }

// Sets a dictionary as a vaule to a key in specific dictionary
- (void)setKey:(NSString*)key ToDictionaryRefValue:(NSString*)dictionaryRefToAdd InDictionary:(NSString*)dictionaryToAddTo
  {
    // Get dictionary to add key and value to, creates one if doesn't exist
    NSMutableDictionary* dictionaryToTransferTo = [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] getReferenceDictionary:dictionaryToAddTo];

    // Get reference to dictionary that needs to be added
    NSMutableDictionary* dictionaryToBeSetAsValue = [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] getReferenceDictionary:dictionaryRefToAdd AndCreateNewInstance:NO];
    if (!dictionaryToBeSetAsValue) {
      NSLog(@"no dictionary reference by the name of: %@", dictionaryRefToAdd);
      return;
    }

    [dictionaryToTransferTo setObject:dictionaryToBeSetAsValue forKey:key];
  }

// Helper function to check if a dictionary is defined and if not creates one
- (NSMutableDictionary*)getReferenceDictionary:(NSString*) dictionaryName
  {
    return [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] getReferenceDictionary:dictionaryName AndCreateNewInstance:YES];
  }

// Helper function to check if a dictionary is defined and will or will not create a new instance if one doesn't exsits
- (NSMutableDictionary*)getReferenceDictionary:(NSString*) dictionaryName AndCreateNewInstance:(BOOL) newInstance
  {
    NSMutableDictionary* currentDictionary = [cSharpDictionaryRefs_ objectForKey:dictionaryName];

    if (!currentDictionary && newInstance)
    {
      currentDictionary = [[NSMutableDictionary alloc] init];
      [cSharpDictionaryRefs_ setObject:currentDictionary forKey: dictionaryName];
    }
    return currentDictionary;
  }

@end

// Converts C style string to NSString
NSString* tjCreateNSString (const char* string)
{
	if (string)
		return [NSString stringWithUTF8String: string];
	else
		return @"";
}

char* tjDuplicateString(const char* string)
{
	if (string == NULL)
		return NULL;
	char* dup = (char*)malloc(strlen(string) + 1);
	strcpy(dup, string);
	return dup;
}
// When native code plugin is implemented in .mm / .cpp file, then functions
// should be surrounded with extern "C" block to conform C function naming rules
extern "C" {
	void Tapjoy_Connect(const char* sdkKey)
	{
		[[Tapjoy sharedTapjoyConnect] setPlugin:@"unity"];

		NSDictionary* connectFlags = [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] getReferenceDictionary:CONNECT_FLAG_KEY];
		if (connectFlags)
			[Tapjoy connect:tjCreateNSString(sdkKey) options:connectFlags];

		else
			[Tapjoy connect:tjCreateNSString(sdkKey)];
	}

	const char* Tapjoy_GetSDKVersion() {
		return tjDuplicateString([Tapjoy getVersion].UTF8String);
	}

	void Tapjoy_ActionComplete(const char* actionID) {
		[Tapjoy actionComplete:tjCreateNSString(actionID)];
	}

	void Tapjoy_SetDebugEnabled(bool enabled)
	{
		[Tapjoy setDebugEnabled:enabled];
		[Tapjoy enableLogging:enabled];
	}

	void Tapjoy_GetPrivacyPolicy()
	{
		if (privacyPolicyInstance == nil) {
			privacyPolicyInstance = [Tapjoy getPrivacyPolicy];
		}
	}

	void Tapjoy_SetUSPrivacy(const char* privacyConsent)
	{
		if (privacyPolicyInstance != nil) {
			[privacyPolicyInstance setUSPrivacy: tjCreateNSString(privacyConsent)];
		}
	}

    void Tapjoy_SetSubjectToGDPRStatus(int gdprApplicable)
    {
        if (privacyPolicyInstance != nil) {
            [privacyPolicyInstance setSubjectToGDPRStatus: (gdprApplicable == 1) ? TJStatusTrue : (gdprApplicable == 0) ? TJStatusFalse : TJStatusUnknown];
        }
    }

    void Tapjoy_SetUserConsentStatus(int consent)
    {
        if (privacyPolicyInstance != nil) {
            [privacyPolicyInstance setUserConsentStatus: (consent == 1) ? TJStatusTrue : (consent == 0) ? TJStatusFalse : TJStatusUnknown];
        }
    }

    void Tapjoy_SetBelowConsentAgeStatus(int isBelowConsentAge)
    {
        if (privacyPolicyInstance != nil) {
            [privacyPolicyInstance setBelowConsentAgeStatus: (isBelowConsentAge == 1) ? TJStatusTrue : (isBelowConsentAge == 0) ? TJStatusFalse : TJStatusUnknown];
        }
    }

    int Tapjoy_GetSubjectToGDPRStatus()
    {
        if (privacyPolicyInstance != nil) {
            return (int)privacyPolicyInstance.subjectToGDPRStatus;
        }
        return (int)TJStatusUnknown;
    }

    int Tapjoy_GetUserConsentStatus()
    {
        if (privacyPolicyInstance != nil) {
            return (int)privacyPolicyInstance.userConsentStatus;
        }
        return (int)TJStatusUnknown;
    }

    int Tapjoy_GetBelowConsentAgeStatus()
    {
        if (privacyPolicyInstance != nil) {
            return (int)privacyPolicyInstance.belowConsentAgeStatus;
        }

        return (int)TJStatusUnknown;
    }

    const char* Tapjoy_GetUSPrivacy()
    {
        if (privacyPolicyInstance != nil) {
            return tjDuplicateString([privacyPolicyInstance USPrivacy].UTF8String);
        }
        return nil;
    }

    void Tapjoy_OptOutAdvertisingID(bool optOut)
    {
        // do nothing
    }

    void Tapjoy_SetUnityVersion(const char* version)
    {
        // TODO: Set unity version
    }

    void Tapjoy_GetCurrencyBalance(void)
	{
		[Tapjoy getCurrencyBalanceWithCompletion:^(NSDictionary *parameters, NSError *error) {
			[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] createCurrencyCallback:@"OnGetCurrencyBalanceResponse" withParameters:parameters andError:error];
		}];
	}

	void Tapjoy_SpendCurrency(int amount)
	{
		[Tapjoy spendCurrency:amount completion:^(NSDictionary *parameters, NSError *error) {
			[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] createCurrencyCallback:@"OnSpendCurrencyResponse" withParameters:parameters andError:error];
		}];
	}

	void Tapjoy_AwardCurrency(int amount)
	{
		[Tapjoy awardCurrency:amount completion:^(NSDictionary *parameters, NSError *error) {
			[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] createCurrencyCallback:@"OnAwardCurrencyResponse" withParameters:parameters andError:error];
		}];
	}

	const char* Tapjoy_GetSupportURL() {
		return tjDuplicateString([Tapjoy getSupportURL].UTF8String);
	}

	const char* Tapjoy_GetSupportURL2(const char* currencyID) {
		return tjDuplicateString([Tapjoy getSupportURL: tjCreateNSString(currencyID)].UTF8String);
	}

	void Tapjoy_ShowDefaultEarnedCurrencyAlert(void)
	{
		// Pops up a UIAlert notifying the user that they have successfully earned some currency.
		// This is the default alert, so you may place a custom alert here if you choose to do so.
		[Tapjoy showDefaultEarnedCurrencyAlert];
	}

	//#pragma mark - Tapjoy Event C Layer

	void Tapjoy_CreatePlacement(const char* guid, const char* name)
	{
		[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] createPlacement:tjCreateNSString(guid) withName:tjCreateNSString(name)];
	}

	void Tapjoy_DismissPlacementContent()
	{
		[TJPlacement dismissContent];
	}

	void Tapjoy_RequestPlacementContent(const char* guid)
	{
		[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] requestPlacementContentWithGuid:tjCreateNSString(guid)];
	}

	void Tapjoy_ShowPlacementContent(const char* guid)
	{
		[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] showPlacementContentWithGuid:tjCreateNSString(guid)];
	}

	bool Tapjoy_IsPlacementContentAvailable(const char* guid)
	{
		return [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] isPlacementContentAvailable:tjCreateNSString(guid)];
	}

	bool Tapjoy_IsPlacementContentReady(const char* guid)
	{
		return [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] isPlacementContentReady:tjCreateNSString(guid)];
	}

    void Tapjoy_SetEntryPoint(const char* guid, const char* entrypoint)
    {
        [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] setEntryPoint:tjCreateNSString(guid) value:tjCreateNSString(entrypoint)];
	}

    void Tapjoy_SetPlacementBalance(const char* guid, const char* currencyId, NSInteger amount)
    {
        return [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] setPlacementBalance:amount forCurrencyId:tjCreateNSString(currencyId) guid:tjCreateNSString(guid)];
    }

    NSInteger Tapjoy_GetPlacementBalance(const char* guid, const char* currencyId)
    {
        return [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] getPlacementBalanceForCurrencyId:tjCreateNSString(currencyId) guid:tjCreateNSString(guid)];
    }

	 void Tapjoy_SetRequiredAmount(const char* guid, const char* currencyId, NSInteger amount)
    {
        return [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] setRequiredAmount:amount forCurrencyId:tjCreateNSString(currencyId) guid:tjCreateNSString(guid)];
    }

    NSInteger Tapjoy_GetRequiredAmount(const char* guid, const char* currencyId)
    {
        return [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] getRequiredAmountForCurrencyId:tjCreateNSString(currencyId) guid:tjCreateNSString(guid)];
    }

    void Tapjoy_RequestOfferwallDiscover(const char* placementName, float height)
    {
        [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] requestOfferwallDiscover:tjCreateNSString(placementName) height:(CGFloat) height];
    }

    void Tapjoy_RequestOfferwallDiscoverAtPosition(const char* placementName, float left, float top, float width, float height)
    {
        [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] requestOfferwallDiscover:tjCreateNSString(placementName) left:(CGFloat) left top:(CGFloat) top width:(CGFloat) width height:(CGFloat) height];
    }

    void Tapjoy_ShowOfferwallDiscover()
    {
        [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] showOfferwallDiscover];
    }

    void Tapjoy_DestroyOfferwallDiscover()
    {
        [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] destroyOfferwallDiscover];
    }

	void Tapjoy_ActionRequestCompleted(const char* requestId)
	{
		[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] actionRequestCompleted:tjCreateNSString(requestId)];
	}

	void Tapjoy_ActionRequestCancelled(const char* requestId)
	{
		[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] actionRequestCancelled:tjCreateNSString(requestId)];
	}

	void Tapjoy_RemovePlacement(const char* guid)
	{
		[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] removePlacement:tjCreateNSString(guid)];
	}

	void Tapjoy_RemoveActionRequest(const char* requestId)
	{
		[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] removeActionRequest:tjCreateNSString(requestId)];
	}

	/* User */
	void Tapjoy_SetUserID(const char* userID)
	{
		[Tapjoy setUserIDWithCompletion:tjCreateNSString(userID) completion:^(BOOL success, NSError *error) {
			if (success) {
				UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeSetUserIDCallback", "OnSetUserIDSuccess");
			} else {
    			NSMutableString *errorMessage = @"OnSetUserIDFailure".mutableCopy;
     			if (error != nil) {
            		[errorMessage appendFormat:@",%ld,%@", error.code, error.localizedDescription];
        		} else {
            		[errorMessage appendFormat:@",%ld,%@", -1, "Unknown Error setting user id."];
        		}
                UnitySendMessage([UNITY_GAME_OBJECT_NAME UTF8String], "OnNativeSetUserIDCallback", [errorMessage UTF8String]);
			}
		}];
	}

	const char* Tapjoy_GetUserID()
	{
		return tjDuplicateString([Tapjoy getUserID].UTF8String);
	}

	/* Currency Callback Param */
	void Tapjoy_SetCustomParameter(const char* customParam)
	{
		[Tapjoy setCustomParameter:tjCreateNSString(customParam)];
	}

	const char* Tapjoy_GetCustomParameter()
	{
		return tjDuplicateString([Tapjoy getCustomParameter].UTF8String);
	}

	void Tapjoy_SetUserLevel(int userLevel)
	{
		[Tapjoy setUserLevel:userLevel];
	}

	int Tapjoy_GetUserLevel()
	{
		return [Tapjoy getUserLevel];
	}

	void Tapjoy_SetMaxLevel(int maxUserLevel)
	{
		[Tapjoy setMaxLevel:maxUserLevel];
	}

	int Tapjoy_GetMaxLevel()
	{
		return [[Tapjoy getMaxLevel] intValue];
	}

	void Tapjoy_SetUserSegment(int userSegment) 
	{  
		switch (userSegment) {
			case 0:
				[Tapjoy setUserSegment:TJSegmentNonPayer];
				break;
			case 1:
				[Tapjoy setUserSegment:TJSegmentPayer];
				break;
			case 2:
				[Tapjoy setUserSegment:TJSegmentVIP];
				break;
			default:
				[Tapjoy setUserSegment:TJSegmentUnknown];
				break;
		}
	}

	int Tapjoy_GetUserSegment()
	{
		return (int)[Tapjoy getUserSegment];
	}

	double Tapjoy_GetScreenScale()
	{
		return (double)[UIScreen mainScreen].scale;
	}

        /* User Tags */
	void Tapjoy_ClearUserTags()
	{
		[Tapjoy clearUserTags];
	}

  const char* Tapjoy_GetUserTags()
  {
    NSError* error = nil;
    NSData *data = [NSJSONSerialization dataWithJSONObject:[[Tapjoy getUserTags] allObjects] options:NSJSONWritingPrettyPrinted error:&error];
    NSString *serializedTags = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];

    return tjDuplicateString(serializedTags.UTF8String);
  }

	void Tapjoy_AddUserTag(const char* tag)
	{
		[Tapjoy addUserTag:tjCreateNSString(tag)];
	}

	void Tapjoy_RemoveUserTag(const char* tag)
	{
		[Tapjoy removeUserTag:tjCreateNSString(tag)];
	}


	/* Track Purchase */
	void Tapjoy_TrackPurchase(const char* currencyCode, double price)
	{
		[Tapjoy trackPurchaseWithCurrencyCode: tjCreateNSString(currencyCode) price: price];
	}

	// Bridge methods between Objective-C and C#
	void Tapjoy_SetKeyToValueInDictionary(const char* key, const char* value, const char* dictionaryToAddTo)
	{
		[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] setKey:tjCreateNSString(key) ToValue:tjCreateNSString(value) InDictionary:tjCreateNSString(dictionaryToAddTo)];
	}

	void Tapjoy_SetKeyToDictionaryRefValueInDictionary(const char* key, const char* dictionaryRefToAdd, const char* dictionaryToAddTo)
	{
		[[TapjoyConnectPlugin sharedTapjoyConnectPlugin] setKey:tjCreateNSString(key) ToDictionaryRefValue:tjCreateNSString(dictionaryRefToAdd) InDictionary:tjCreateNSString(dictionaryToAddTo)];
	}
}
