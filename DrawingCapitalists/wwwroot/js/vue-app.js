var app = new Vue({
    el: '#app',
    data: {
        views: [],
        currentView: null
    },
    mounted() {
        window.addEventListener('popstate', e => {
            console.log(e);
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
                str += data.messages[i] + "\n";
            }

            if (data.messageType == 0)
                this.showCommonMessage(str);
            else if (data.messageType == 1)
                this.showErrorMessage(str);
            else if (data.messageType == 2)
                this.showSuccessMessage(str);
        },
        showErrorMessage: function (str) { //messageType = 1
            alert("Error " + str);
        },
        showSuccessMessage: function (str) { //messageType = 2
            alert("Success " + str);
        },
        showCommonMessage: function (str) { //messageType = 0
            alert("Common " + str);
        },
        setLocation: function (page) {
            history.pushState(null, null, '/' + page);
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