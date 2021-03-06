﻿var app = new Vue({
    el: '#app',
    data: {
        views: [],
        currentView: null,
        alerts: [],
        menu: {
            isOpen: false,
            isShow: false,
            userName: null,
            userAva: null
        },
        paramsObject: null
    },
    mounted() {
        window.addEventListener('popstate', e => {
            let pathName = this.getLocation();
            this.switchPage(pathName);
        });

        this.httpGet('/Authentication/GetUserInfo',
            response => {
                if (response.data) {
                    let userInfo = response.data;
                    this.initUserMenuData(userInfo.base64Ava, userInfo.name);
                }
            });
    },
    methods: {
        switchPage: function (page) {
            let normPageName = page.split('?')[0].toLowerCase();
            let validUrl = page.includes('?') ? page + "&" : page + "?";

            if (!this.views.includes(normPageName)) {
                this.httpGet(`/${validUrl}useLayout=false`,
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
            let str = "";
            let lens = data.messages.length;

            for (let i = 0; i < lens - 1; ++i) {                
                str += data.messages[i] + "<br><br>";
            }

            if (lens > 0)
                str += data.messages[lens - 1] + "<br>";

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
            let obj = {
                message: str,
                mtype: alertType 
            };

            this.alerts.push(obj);

            setTimeout(() => {
                this.removeAlert(obj);
            }, 10000);
        },
        removeAlert: function (obj) {
            let i = this.alerts.indexOf(obj);
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
            let curPathName = this.getLocation();
            if (curPathName != page)
                history.pushState(null, null, '/' + page);
        },
        getLocation: function () {
            let pathName = window.location.pathname;
            if (pathName.length > 1) {
                pathName = pathName.substring(1, pathName.length).split('?')[0];
            }
            else {
                pathName = '';
            }

            return pathName;
        },
        getLocationParam: function (param) {
            let url = new URL(window.location.href);
            return url.searchParams.get(param);
        },
        createHubConnection: function (url) {
            let hubConnection = new signalR.HubConnectionBuilder()
                .withUrl(url)
                .build();

            hubConnection.on('SwitchPage', this.switchPage);
            hubConnection.on('ShowClientMessage', this.showClientMessage);

            return hubConnection;
        },
        initUserMenuData: function (ava, name) {
            this.menu.userAva = ava;
            this.menu.userName = name;
        },
        openCloseUserMenu: function () {
            this.menu.isOpen = !this.menu.isOpen;
        },
        showHideUserMenu: function (show) {
            this.menu.isShow = show;
        },
        onLoginMenuClick: function () {
            this.menu.isOpen = false;
            this.menu.isShow = false;

            this.switchPage('login');          
        },
        onLobbyMenuClick: function () {
            this.switchPage('hub');  
        },
        isFireFox: function () {
            let ua = navigator.userAgent;
            return ua.search(/Firefox/) > 0;
        }
    }
})