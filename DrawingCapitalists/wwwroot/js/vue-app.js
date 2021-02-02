var app = new Vue({
    el: '#app',
    data: {
        views: [],
        currentView: null,
        alerts: []
    },
    mounted() {
        window.addEventListener('popstate', e => {
            var pathName = this.getLocation();
            this.switchPage(pathName);
        });
    },
    methods: {
        switchPage: function (page) {
            var normPageName = page.toLowerCase();

            if (!this.views.includes(normPageName)) {
                this.httpGet(`/${page}?useLayout=false`,
                    response => {
                        //console.log(response);
                        $("#pages").append(response.data);
                        this.views.push(normPageName);
                        this.currentView = normPageName;
                        this.setLocation(page);
                    },
                    ex => {
                        //console.log(ex);
                        if (this.views.includes('login')) {
                            this.currentView = 'login';
                            this.setLocation('login');
                        }
                        else {
                            this.switchPage('login');
                        }
                    });
            }
            else {
                this.currentView = normPageName;
                this.setLocation(page);
            }            
        },
        httpGet: function (url, thenClbk, catchClbk) {
            axios.get(url)
                .then(response => {
                    thenClbk(response);
                })
                .catch(error => {
                    if (catchClbk)
                        catchClbk(error);

                    if (error.response.data)
                        this.showClientMessage(error.response.data);
                });
        },
        httpPost: function (url, obj, thenClbk, catchClbk) {
            axios.post(url, obj)
                .then(response => {
                    thenClbk(response);
                })
                .catch(error => {
                    if (catchClbk)
                        catchClbk(error);

                    if (error.response.data)
                        this.showClientMessage(error.response.data);
                })
        },
        showClientMessage: function (data) {
            var str = "";

            for (var i = 0; i < data.messages.length; ++i) {                
                str += data.messages[i] + "<br>";
            }

            this.addAlert(str, data.messageType);
        },
        showCommonMessage: function (str) { //messageType = 0
            this.addAlert(str, 0);
        },
        showErrorMessage: function (str) { //messageType = 1
            this.addAlert(str, 1);
        },
        showSuccessMessage: function (str) { //messageType = 2
            this.addAlert(str, 2);
        },
        addAlert: function (str, alertType) {
            var obj = {
                message: str,
                mtype: alertType 
            };

            this.alerts.push(obj);

            setTimeout(() => {
                this.removeAlert(obj);
            }, 10000);
        },
        removeAlert: function (obj) {
            var i = this.alerts.indexOf(obj);
            if (i > -1)
                this.alerts.splice(i, 1);
        },
        getColorForMType: function (mtype) {
            if (mtype == 0)
                return 'var(--blue-col)';
            else if (mtype == 1)
                return 'var(--red-col)';
            else if (mtype == 2)
                return 'var(--gold-col)';
        },
        setLocation: function (page) {
            var curPathName = this.getLocation();
            if (curPathName != page)
                history.pushState(null, null, '/' + page);
        },
        getLocation: function () {
            var pathName = window.location.pathname;
            if (pathName.length > 1) {
                pathName = pathName.substring(1, pathName.length).split('?')[0];
            }
            else {
                pathName = '';
            }

            return pathName;
        },
        createHubConnection: function (url) {
            var hubConnection = new signalR.HubConnectionBuilder()
                .withUrl(url)
                .build();

            hubConnection.on('ShowClientMessage', this.showClientMessage);

            return hubConnection;
        }
    }
})