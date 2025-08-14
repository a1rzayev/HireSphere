const AuthUtils = {
    isLoggedIn: function() {
        return localStorage.getItem('accessToken') !== null;
    },

    getAccessToken: function() {
        return localStorage.getItem('accessToken');
    },

    getRefreshToken: function() {
        return localStorage.getItem('refreshToken');
    },



    logout: function() {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        console.log('Tokens cleared from localStorage');
    },

    isTokenExpired: function() {
        const token = this.getAccessToken();
        if (!token) return true;
        
        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            const currentTime = Math.floor(Date.now() / 1000);
            return payload.exp < currentTime;
        } catch (e) {
            return true;
        }
    }
};

document.addEventListener('DOMContentLoaded', function() {
    if (AuthUtils.isLoggedIn()) {
        console.log('User is logged in with valid tokens');
    } else {
        console.log('User is not logged in');
    }
});

window.addEventListener('beforeunload', function() {
    if (AuthUtils.isLoggedIn()) {
        console.log('Page unloading - clearing tokens');
        AuthUtils.logout();
    }
});
