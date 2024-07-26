//
//  TapjoyOfferwallDiscoverPlugin.m
//  UnityFramework
//
//  Created by Dominic Roberts on 24/06/2022.
//

#import "TapjoyOfferwallDiscoverPlugin.h"
#import "TapjoyConnectPlugin.h"

@interface TapjoyOfferwallDiscoverPlugin()
@property (nonatomic, retain) TapjoyPluginAPI* tapjoyApiPlugin;

@end

@implementation TapjoyOfferwallDiscoverPlugin

- (id)init
{
    self = [super init];
    
    if (self)
    {
        [self setTapjoyApiPlugin:[[TapjoyPluginAPI alloc] init]];
    }

    return self;
}

- (void)request:(NSString *)placementName height:(CGFloat)height {
    [[self tapjoyApiPlugin] requestOfferwallDiscover:placementName height:(CGFloat)height delegate:self];
}

- (void)request:(NSString *)placementName left:(CGFloat)left top:(CGFloat)top width:(CGFloat)width height:(CGFloat)height {
    [[self tapjoyApiPlugin] requestOfferwallDiscover:placementName left:(CGFloat)left top:(CGFloat)top width:(CGFloat)width height:(CGFloat)height delegate:self];
}

-(void)show:(UIView*)view {
    [[self tapjoyApiPlugin] showOfferwallDiscover:view];
}

-(void)destroy {
    [[self tapjoyApiPlugin] destroyOfferwallDiscover];
}

- (void)requestDidSucceedForView:(TJOfferwallDiscoverView *)view {
    [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] offerwallDiscoverRequestDidSucceed];
}

- (void)requestDidFailForView:(TJOfferwallDiscoverView *)view error:(nullable NSError *)error {
    [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] offerwallDiscoverRequestDidFail:error];
}

- (void)contentIsReadyForView:(TJOfferwallDiscoverView *)view {
    [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] offerwallDiscoverContentReady];
}

- (void)contentErrorForView:(TJOfferwallDiscoverView *)view error:(nullable NSError *)error {
    [[TapjoyConnectPlugin sharedTapjoyConnectPlugin] offerwallDiscoverContentError:error];
}
@end
