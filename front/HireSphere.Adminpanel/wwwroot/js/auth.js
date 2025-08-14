const AuthUtils = {
    isLoggedIn: function() {
        return this.validateTokens();
    },

    getAccessToken: function() {
        return localStorage.getItem('accessToken');
    },

    getRefreshToken: function() {
        return localStorage.getItem('refreshToken');
    },

    logout: async function() {
        console.log('Logout function called - starting logout process...');
        
        try {
            const refreshToken = this.getRefreshToken();
            
            if (refreshToken) {
                const baseUrl = 'https://localhost:7001'; 
                const response = await fetch(`${baseUrl}/api/auth/logout`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify(refreshToken)
                });
                
                if (response.ok) {
                    console.log('Backend logout successful');
                } else {
                    console.log('Backend logout failed, but continuing with frontend cleanup');
                }
            }
        } catch (error) {
            console.log('Error calling backend logout, but continuing with frontend cleanup:', error);
        }
        
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        console.log('Admin tokens cleared from localStorage');
        
        updateNavigationVisibility();
        
        window.location.reload();
    },

    isTokenExpired: function() {
        const token = this.getAccessToken();
        if (!token) return true;
        
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const currentTime = Math.floor(Date.now() / 1000);
            const isExpired = payload.exp < currentTime;
            console.log('Token expiration check:', { 
                currentTime, 
                tokenExpiry: payload.exp, 
                isExpired 
            });
            return isExpired;
        } catch (e) {
            console.error('Error checking token expiration:', e);
            return true;
        }
    },

    validateTokens: function() {
        const hasAccessToken = !!this.getAccessToken();
        const hasRefreshToken = !!this.getRefreshToken();
        const isExpired = this.isTokenExpired();
        
        console.log('Token validation:', {
            hasAccessToken,
            hasRefreshToken,
            isExpired,
            isValid: hasAccessToken && hasRefreshToken && !isExpired
        });
        
        return hasAccessToken && hasRefreshToken && !isExpired;
    }
};

if (window.authInitialized) {
    console.log('Auth already initialized, skipping...');
} else {
    window.authInitialized = true;
    
    document.addEventListener('DOMContentLoaded', function() {
        console.log('DOM Content Loaded - Checking authentication status...');
        console.log('localStorage accessToken:', localStorage.getItem('accessToken') ? 'Present' : 'Not present');
        console.log('localStorage refreshToken:', localStorage.getItem('refreshToken') ? 'Present' : 'Not present');
        
        updateNavigationVisibility();
        
        if (AuthUtils.isLoggedIn()) {
            console.log('Admin user is logged in with valid tokens');
        } else {
            console.log('Admin user is not logged in');
        }
    });
}

let navigationUpdateCount = 0;
let lastNavigationState = null;

function updateNavigationVisibility() {
    navigationUpdateCount++;
    console.log(`updateNavigationVisibility called ${navigationUpdateCount} times`);
    
    const loginNavItem = document.getElementById('login-nav-item');
    const logoutNavItem = document.getElementById('logout-nav-item');
    const adminNavItems = document.getElementById('admin-nav-items');
    
    const currentState = AuthUtils.isLoggedIn() ? 'logged-in' : 'logged-out';
    
    if (lastNavigationState === currentState) {
        console.log(`Navigation state unchanged (${currentState}), skipping update`);
        return;
    }
    
    lastNavigationState = currentState;
    
    if (AuthUtils.isLoggedIn()) {
        if (loginNavItem) loginNavItem.style.display = 'none';
        if (logoutNavItem) logoutNavItem.style.display = 'block';
        if (adminNavItems) adminNavItems.style.display = 'block';
        console.log('Navigation updated: User is logged in');
    } else {
        if (loginNavItem) loginNavItem.style.display = 'block';
        if (logoutNavItem) logoutNavItem.style.display = 'none';
        if (adminNavItems) adminNavItems.style.display = 'none';
        console.log('Navigation updated: User is not logged in');
    }
}

console.log('Automatic token clearing disabled - tokens will persist across page reloads');
