// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

var mapOptions;
var map;
var geoSearchResults;
var markers = [];
google.maps.event.addDomListener(window, 'load', loadMap);

function loadMap() {

    mapOptions = {
        center: new google.maps.LatLng(17.609993, 83.221436),
        zoom: 12,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };

    map = new google.maps.Map(document.getElementById("googlemap"), mapOptions);
}


function addMarker(location, iwindow) {
    var marker = new google.maps.Marker({
        position: location,
        map: map
    });
    marker.addListener('click', function () {
        iwindow.open(map, marker);
    });
    markers.push(marker);
}

function updateMap() {
    var a = document.getElementById('textinput').value;
    fetch('https://nominatim.openstreetmap.org/search?q=' + a + '&format=json')
        .then(response => {
            return response.json()
        })
        .then(data => {
            geoSearchResults = data;
            var firstHit = geoSearchResults[0];
            if (firstHit == null) {
                alert("No search results available");
                return;
            }
            for (var i = 0; i < markers.length; i++) {
                markers[i].setMap(null);
            }
            markers = [];

            var bb = firstHit.boundingbox;
            if (Math.abs(bb[0] - bb[1])>0.01) { //This checks that the bounding box is not a point
                map.fitBounds(new google.maps.LatLngBounds(new google.maps.LatLng(bb[0], bb[2]), new google.maps.LatLng(bb[1], bb[3])))
            }
            else {// if the bounding box is a point then default to a pan to long/lat
                map.panTo(new google.maps.LatLng(firstHit.lat, firstHit.lon));
            }

            var infowindow = new google.maps.InfoWindow({
                content: firstHit.display_name
            });
            addMarker(new google.maps.LatLng(firstHit.lat, firstHit.lon), infowindow);

            //Now to post the search to the server so it can be logged.
            $.ajax({
                url: '@Url.Action("Log", "Home")',
                type: 'POST',
                dataType: 'json',
                // we set cache: false because GET requests are often cached by browsers
                // IE is particularly aggressive in that respect
                cache: false,
                data: { search: a, firstHit: JSON.stringify(firstHit) },
                success: function () {
                    alert("Submission successful");
                },
                error: function (error) {
                    alert("Unable to post data to log! \n" + error.responseJSON.message)
                }
            });
        })

}