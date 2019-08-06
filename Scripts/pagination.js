function createPagination(pages, page, path) {
	let str = '<ul>';
	let active;
	let pageCutLow = page - 1;
	let pageCutHigh = page + 1;
	// Show the Previous button only if you are on a page other than the first
	if (page > 1) {
		str += '<a onclick="createPagination(' + pages + ', ' + (page - 1) + ', ' + path + ')" href="' + path + (page - 1) + '" title="Previous">&#10094</a>';
	}
	// Show all the pagination elements if there are less than 6 pages total
	if (pages < 6) {
		for (let p = 1; p <= pages; p++) {
			active = page == p ? "active" : "no";
			str += '<a onclick="createPagination(' + pages + ', ' + p + ', ' + path + ')" href="' + path + p + '" class="' + active + '">' + p + '</a>';
		}
	}
	// Use "..." to collapse pages outside of a certain range
	else {
		// Show the very first page followed by a "..." at the beginning of the
		// pagination section (after the Previous button)
		if (page > 2) {
			str += '<a onclick="createPagination(' + pages + ', 1)" href="' + path + '1">1</a>';
			if (page > 3) {
				str += '<a onclick="createPagination(' + pages + ',' + (page - 2)  + ', ' + path + ')" href="' + path + (page - 2) + '">...</a>';
			}
		}
		// Determine how many pages to show after the current page index
		if (page === 1) {
			pageCutHigh += 2;
		} else if (page === 2) {
			pageCutHigh += 1;
		}
		// Determine how many pages to show before the current page index
		if (page === pages) {
			pageCutLow -= 2;
		} else if (page === pages - 1) {
			pageCutLow -= 1;
		}
		// Output the indexes for pages that fall inside the range of pageCutLow
		// and pageCutHigh
		for (let p = pageCutLow; p <= pageCutHigh; p++) {
			if (p === 0) {
				p += 1;
			}
			if (p > pages) {
				continue
			}
			active = page == p ? "active" : "no";
			str += '<a onclick="createPagination(' + pages + ', ' + p + ', ' + path + ')" href="' + path + p + '" class="' + active + '">' + p + '</a>';
		}
		// Show the very last page preceded by a "..." at the end of the
		// pagination
		// section (before the Next button)
		if (page < pages - 1) {
			if (page < pages - 2) {
				str += '<a onclick="createPagination(' + pages + ',' + (page + 2) + ', ' + path + ')" href="' + path + (page + 2) + '">...</a>';
			}
			str += '<a onclick="createPagination(' + pages + ', ' + pages + ', ' + path + ')" href="' + path + pages + '">' + pages + '</a>';
		}
	}
	// Show the Next button only if you are on a page other than the last
	if (page < pages) {
		str += '<a onclick="createPagination(' + pages + ', ' + (page + 1) + ', ' + path + ')" href="' + path + (page + 1) + '" title="Next">&#10095</a>';
	}
	str += '</ul>';
	// Return the pagination string to be outputted in the pug templates
	document.getElementById('pagination').innerHTML = str;
	return page;
}