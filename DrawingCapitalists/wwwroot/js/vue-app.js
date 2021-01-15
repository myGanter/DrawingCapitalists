var app = new Vue({
    el: '#app',
    data: {
        views: [],
        currentView: null
    },
    methods: {
        switchPage: function (page) {
            if (!this.views.includes(page)) {
                this.httpGet(`/?page=${page}&useLayout=false`,
                    response => {
                        //console.log(response);
                        $("#pages").append(response.data);
                        this.views.push(page);
                        this.currentView = page;
                        this.setLocation(page);
                    });
            }
            else {
                this.currentView = page;
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

                    if (error.data)
                        showClientMessage(error.data);
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

                    if (error.data)
                        showClientMessage(error.data);
                })
        },
        showClientMessage: function (data) {
            var str = "";

            for (var i in data.Messages)
                str += i + "\n";

            alert(str);
        },
        setLocation: function (page) {
            history.pushState(null, null, '/' + page);
        }
    }
})