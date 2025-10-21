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
        
        // Clear both localStorage and session
        this.clearAllTokens();
        
        // Update navigation
        updateNavigationVisibility();
        
        // Redirect to login page
        window.location.href = '/Auth/Login';
    },

    clearAllTokens: function() {
        // Clear localStorage
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        console.log('localStorage tokens cleared');
        
        // Clear session by calling logout endpoint
        fetch('/Auth/Logout', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            }
        }).then(() => {
            console.log('Session cleared via logout endpoint');
        }).catch(error => {
            console.log('Error clearing session:', error);
        });
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

// Initialize authentication on page load
document.addEventListener('DOMContentLoaded', function() {
    console.log('Auth initialization - checking authentication status...');
    console.log('localStorage accessToken:', localStorage.getItem('accessToken') ? 'Present' : 'Not present');
    console.log('localStorage refreshToken:', localStorage.getItem('refreshToken') ? 'Present' : 'Not present');
    
    updateNavigationVisibility();
    
    if (AuthUtils.isLoggedIn()) {
        console.log('Admin user is logged in with valid tokens');
    } else {
        console.log('Admin user is not logged in');
    }
});

function updateNavigationVisibility() {
    const loginNavItem = document.getElementById('login-nav-item');
    const logoutNavItem = document.getElementById('logout-nav-item');
    const adminNavItems = document.getElementById('admin-nav-items');
    
    if (!loginNavItem || !logoutNavItem || !adminNavItems) {
        console.log('Navigation elements not found, skipping update');
        return;
    }
    
    const isLoggedIn = AuthUtils.isLoggedIn();
    
    if (isLoggedIn) {
        loginNavItem.style.display = 'none';
        logoutNavItem.style.display = 'block';
        adminNavItems.style.display = 'block';
        console.log('Navigation updated: User is logged in');
    } else {
        loginNavItem.style.display = 'block';
        logoutNavItem.style.display = 'none';
        adminNavItems.style.display = 'none';
        console.log('Navigation updated: User is not logged in');
    }
}

console.log('Auth utilities loaded and ready');
