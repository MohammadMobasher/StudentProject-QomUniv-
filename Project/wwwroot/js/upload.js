$('.upload-btn').on('click', function (){
    $('#uploadbtn').click();
    $('.progress-bar').text('0%');
    $('.progress-bar').width('0%');
});

$('#uploadbtn').on('change', function(){

  var files = $(this).get(0).files;

  if (files.length > 0){

    var formData = new FormData();

    for (var i = 0; i < files.length; i++) {
      var file = files[i];

      formData.append('uploads[]', file, file.name);
    }

    