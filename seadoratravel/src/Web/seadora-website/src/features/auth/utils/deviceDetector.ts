export const isAppleDevice = (): boolean => {
  if (typeof window === 'undefined') return false;
  const ua = window.navigator.userAgent.toLowerCase();
  
  // Check user agent for Apple devices
  const isAppleUserAgent = /iphone|ipad|ipod|mac/.test(ua);
  
  // Extra check for Safari
  const isSafari = /^((?!chrome|android).)*safari/i.test(ua);
  
  // Check for Apple Pay capability
  const hasApplePay = typeof (window as any).ApplePaySession !== 'undefined';
  
  return isAppleUserAgent || (isSafari && hasApplePay);
};

export const isMobileDevice = (): boolean => {
  if (typeof window === 'undefined') return false;
  
  const ua = window.navigator.userAgent.toLowerCase();
  const isMobileUserAgent = /android|webos|iphone|ipad|ipod|blackberry|iemobile|opera mini/.test(ua);
  const isSmallScreen = window.innerWidth <= 768;
  
  return isMobileUserAgent || isSmallScreen;
};
