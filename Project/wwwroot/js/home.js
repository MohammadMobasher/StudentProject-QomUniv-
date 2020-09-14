<script>
var i = 0;
function move() {
if (i == 0) {
i = 1;
var elem = document.getElementById("myBar");
var width = 10;
var id = setInterval(frame, 10);
function frame() {
  if (width >= 100) {
    clearInterval(id);
    i = 0;
  } else {
    width++;
    elem.style.width = width + "%";
    elem.innerHTML = width  + "%";
  }
}
}
}
</script>
<script>
var x = document.getElementById("myAudio");

function getPlaySpeed() { 
  alert(x.playbackRate);
} 

function setPlaySpeed() { 
  x.playbackRate = 2;
} 
</script>