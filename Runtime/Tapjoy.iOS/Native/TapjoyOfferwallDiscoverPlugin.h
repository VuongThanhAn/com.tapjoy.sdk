//
//  TapjoyOfferwallDiscoverPlugin.h
//  UnityFramework
//
//  Created by Dominic Roberts on 24/06/2022.
//

#import <Foundation/Foundation.h>
#import <Tapjoy/Tapjoy.h>
#import <Tapjoy/TapjoyPluginAPI.h>

NS_ASSUME_NONNULL_BEGIN

@interface TapjoyOfferwallDiscoverPlugin : NSObject<TJOfferwallDiscoverDelegate>

- (void)request:(NSString*)placementName height:(CGFloat)height;
- (void)request:(NSString *)placementName left:(CGFloat)left top:(CGFloat)top width:(CGFloat)width height:(CGFloat)height;
- (void)show:(UIView*)view;
- (void)destroy;

- (void)requestDidSucceedForView:(TJOfferwallDiscoverView *)view;
- (void)requestDidFailForView:(TJOfferwallDiscoverView *)view error:(nullable NSError *)error;
- (void)contentIsReadyForView:(TJOfferwallDiscoverView *)view;
- (void)contentErrorForView:(TJOfferwallDiscoverView *)view error:(nullable NSError *)error;

@end

NS_ASSUME_NONNULL_END
