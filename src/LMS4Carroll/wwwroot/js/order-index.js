(function ($) {
    function Order() {
        var $this = this;

        function initilizeModel() {
            $("#modal-action-order").on('loaded.bs.modal', function (e) {

            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }
        $this.init = function () {
            initilizeModel();
        }
    }
    $(function () {
        var self = new Order();
        self.init();
    })
}(jQuery))
