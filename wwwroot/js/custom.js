
/*-------------------- Movie Import --------------------*/
// Confiugre Bloodhound
let movieList = new Bloodhound({
    datumTokenizer: Bloodhound.tokenizers.whitespace('name', 'value'),
    queryTokenizer: Bloodhound.tokenizers.whitespace,
    remote: {
        url: query,                         // Coming from the razor view
        wildcard: '%SEARCH',                // %SEARCH will be replaced by users input in the url option.
        transform: function (response) {    // This function will run before passing the results back to the page
            let arr = response.results;
            // Sort results by popularity
            arr.sort((a, b) => {
                return a.vote_average > b.vote_average ? -1 : 1;
            });

            let results = [];
            arr.forEach(el => {
                results.push({ name: el.original_title, value: el.id});
            });

            return results;
        }
    }
});

// Set typeahead for the input
$('#movieSearch .typeahead').typeahead(
    {
        hint: true,
        highlight: true,
        minLength: 2,
    },
    {
        name: 'movie-list',
        source: movieList,                  // suggestion engine is passed as the source
        display: 'name',
        limit: 10,
        templates: {
            pending: function (query) {
                return '<div class="tt-suggestion">Loading...</div>';
            }
        }
    }
);

$('.typeahead').on('typeahead:selected', function (evt, item) {
    // do what you want with the item here
    // set the hidden inputs value to the datum value eg. "21"
    $('#movie-id').val(item.value);
})

/*-------------------- Movie Collections --------------------*/
$('#Form').on('submit', onSubmit);

function onSubmit() {
    let collectionIndex = document.getElementById("id").selectedIndex;
    if (collectionIndex == -1) {
        document.getElementById("id").selectedIndex = 0;
        console.log(collectionIndex);
    }

    let select = document.getElementById("idsInCollection").getElementsByTagName("option");

    for (let option of select) {
        option.selected = "selected";
    }
}

$(document).on("click", function (event) {
    let $trigger = $("#idsInCollection, #idsNotInCollection");
    if ($trigger !== event.target && !$trigger.has(event.target).length) {
        $("#idsInCollection, #idsNotInCollection").prop("selectedIndex", -1);
    }
});

$('.CollectionId').on('change', function () {
    let id = $('.CollectionId').val()
    var url = '@Url.Action("Index", "MovieCollections", new { id = "1" })'.split("/");
    url[3] = id;
    url = url.join("/");

    window.location.href = url;
});

$('#addToCollection').click(function (e) {
    var selItem = $('#idsNotInCollection').prop('selectedIndex');
    console.log(selItem);
    if (selItem == -1) {
        //window.alert("You must first select a movie not in the collection.")
    } else {
        // Add to new list
        $('#idsInCollection').append($('#idsNotInCollection')[0][selItem]);
        // Remove from original list
        let selected = $('#idsNotInCollection').val();
        $(`#idsNotInCollection option[value=${selected}]`).remove();
        // Uncheck selected items
        $('#idsNotInCollection').prop('selectedIndex', -1);
        $('#idsInCollection').prop('selectedIndex', -1);
    }
});

$('#removeFromCollection').click(function (e) {
    var selItem = $('#idsInCollection').prop('selectedIndex');
    if (selItem == -1) {
        //window.alert("You must first select a movie in the collection.")
    } else {
        // Add to new list
        $('#idsNotInCollection').append($('#idsInCollection')[0][selItem]);
        // Remove from original list
        let selected = $('#idsInCollection').val();
        $(`#idsInCollection option[value=${selected}]`).remove();
        // Uncheck selected items
        $('#idsNotInCollection').prop('selectedIndex', -1);
        $('#idsInCollection').prop('selectedIndex', -1);
    }
});

$('#moveUpIn').click(function () {
    let arr = $('#idsInCollection').children().toArray();
    let selItem = $('#idsInCollection').prop('selectedIndex');

    if (selItem == -1 || selItem == 0) {
        //window.alert("You must first select a movie in the collection.")
    } else {
        let temp = arr[selItem];
        arr[selItem] = arr[selItem - 1];
        arr[selItem - 1] = temp;

        $('#idsInCollection').empty().append(arr);
    }
});

$('#moveUpNotIn').click(function () {
    let arr = $('#idsNotInCollection').children().toArray();
    let selItem = $('#idsNotInCollection').prop('selectedIndex');

    if (selItem == -1 || selItem == 0) {
        //window.alert("You must first select a movie in the collection.")
    } else {
        let temp = arr[selItem];
        arr[selItem] = arr[selItem - 1];
        arr[selItem - 1] = temp;

        $('#idsNotInCollection').empty().append(arr);
    }
});

$('#moveDownIn').click(function () {
    let arr = $('#idsInCollection').children().toArray();
    let selItem = $('#idsInCollection').prop('selectedIndex');

    if (selItem == -1 || selItem == arr.length - 1) {
        //window.alert("You must first select a movie in the collection.")
    } else {
        let temp = arr[selItem];
        arr[selItem] = arr[selItem + 1];
        arr[selItem + 1] = temp;

        $('#idsInCollection').empty().append(arr);
    }
});

$('#moveDownNotIn').click(function () {
    let arr = $('#idsNotInCollection').children().toArray();
    let selItem = $('#idsNotInCollection').prop('selectedIndex');

    if (selItem == -1 || selItem == arr.length - 1) {
        //window.alert("You must first select a movie in the collection.")
    } else {
        let temp = arr[selItem];
        arr[selItem] = arr[selItem + 1];
        arr[selItem + 1] = temp;

        $('#idsNotInCollection').empty().append(arr);
    }
});