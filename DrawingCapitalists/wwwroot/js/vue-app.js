var app = new Vue({
    el: '#app',
    data: {
        message: 'Привет, Vue!',
        views: [],
        viewName: null,
        currentView: null
    },
    methods: {
        send: function () {
            if (!this.views.includes(this.viewName)) {
                this.views.push(this.viewName);

                axios.get('/?page=' + this.viewName).
                    then(response => {
                        $("#comps").append(response.data);
                        console.log(response);
                    })
                    .catch(error => {
                        console.log(error);
                    })
            }
        },
        swich: function (val) {
            
            this.currentView = val;
        }
    }
})