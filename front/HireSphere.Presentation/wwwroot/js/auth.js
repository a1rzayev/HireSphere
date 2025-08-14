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

    getUserInfo: function() {
        return {
            id: localStorage.getItem('userId'),
            name: localStorage.getItem('userName'),
            email: localStorage.getItem('userEmail'),
            role: localStorage.getItem('userRole')
        };
    },

    logout: function() {
        localStorage.removeItem('accessToken');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('userId');
        localStorage.removeItem('userName');
        localStorage.removeItem('userEmail');
        localStorage.removeItem('userRole');
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
        console.log('User is logged in:', AuthUtils.getUserInfo());
    } else {
        console.log('User is not logged in');
    }
});
