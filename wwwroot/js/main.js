

async function loadCourses() {
    const request = new Request("api/courses");
    const response = await fetch(request);
    const courses = await response.json();
    const container = document.getElementById("courses");
    for (const course of courses) {
        const div = document.createElement("div");
        const label = document.createElement("label");
        const checkbox = document.createElement("input");

        checkbox.id = `course_${course.id}`;
        checkbox.type = "checkbox";
        checkbox.name = "course";
        checkbox.value = course.id;

        label.innerHTML = `${course.name} (${course.length} hours)`;
        label.htmlFor = checkbox.id;

        div.appendChild(checkbox);
        div.appendChild(label);
        container.appendChild(div);
    }
}
async function loadCalculationStrategies() {
    const request = new Request("api/schedules/calculation-strategies");
    const response = await fetch(request);
    const strategies = await response.json();
    const container = document.getElementById("strategies");

    let id = 1;
    for (const strategy of strategies) {
        const div = document.createElement("div");
        const label = document.createElement("label");
        const radio = document.createElement("input");

        radio.id = `strategy_${id}`;
        radio.type = "radio";
        radio.name = "strategy";
        radio.value = strategy.id;
        if (id === 1) {
            radio.checked = true;
        }
        id += 1;

        label.innerHTML = strategy.description;
        label.htmlFor = radio.id;

        div.appendChild(radio);
        div.appendChild(label);
        container.appendChild(div);
    }

}

async function removeSchedule(id, row) {
    const request = new Request(`api/schedules/${id}`, { method: "DELETE" });
    const response = await fetch(request);
    if (response.ok) {
        row.parentNode.removeChild(row);
    }
};

async function loadSchedules() {

    function createCell(content, header) {
        const cell = document.createElement(header ? "th" : "td");
        cell.innerText = content;
        return cell;
    }

    const studentId = parseInt(document.getElementById("studentId").value)
    if (isNaN(studentId)) {
        return;

    }
    const params = new URLSearchParams({ studentId });
    const request = new Request(`api/schedules?${params}`);
    const response = await fetch(request);
    const schedules = await response.json();
    const table = document.getElementById("schedule");
    const header = table.getElementsByTagName("thead")[0];
    const body = table.getElementsByTagName("tbody")[0];
    header.innerHTML = "";
    body.innerHTML = "";
    if (schedules.length === 0) {
        body.innerHTML = "<tr><td>No Schedules</td></tr>";
        return;
    }
    const  maxWeeks = schedules.reduce((acc, el) => Math.max(acc, el.weeklyHours.length), 0);
    const headerFragment = new DocumentFragment();
    const headerRow = document.createElement("tr");
    headerRow.appendChild(createCell("Start Date"), true);
    headerRow.appendChild(createCell("End Date"), true);
    headerRow.appendChild(createCell("Courses"), true);
    for (let i = 1; i <= maxWeeks; ++i) {
        headerRow.appendChild(createCell(`W${i}`), true);
    }
    headerRow.appendChild(createCell(""), true);
    headerFragment.appendChild(headerRow);
    header.appendChild(headerFragment);

    const bodyFragment = new DocumentFragment();

    for (let schedule of schedules) {
        const row = document.createElement("tr");
        row.appendChild(createCell(schedule.startDate.split("T")[0]));
        row.appendChild(createCell(schedule.endDate.split("T")[0]));
        const courses = schedule.courses.map(x => x.name).join(", ");
        const coursesCell = row.appendChild(createCell(courses));
        coursesCell.title = courses;
        for (let i = 0; i < maxWeeks; ++i) {
            const load = i < schedule.weeklyHours.length ? schedule.weeklyHours[i] : "";
            row.appendChild(createCell(load));
        }
        const removeCell = document.createElement("td");
        removeCell.innerHTML = "<span>Remove</span>";
        removeCell.addEventListener('click', () => removeSchedule(schedule.id, row));
        row.appendChild(removeCell);

        bodyFragment.appendChild(row);
    }

    body.appendChild(bodyFragment);

}

function prepareRequest(formData) {
    return {
        studentId: parseInt(formData.get("studentId")),
        startDate: formData.get("startDate"),
        endDate: formData.get("endDate"),
        strategy: formData.get("strategy"),
        courseIds: formData.getAll("course").map(x => parseInt(x))
    }
}

function showError(message) {
    const errorBox = document.getElementById("errorMessage");
    errorBox.style.display = "";
    errorBox.innerText = JSON.stringify(message,null,4);
}

function hideError() {
    document.getElementById("errorMessage").style.display = "none";
}

function showPreview(schedule) {
    const previewBox = document.getElementById("preview");
    previewBox.style.display = "";
    previewBox.innerHTML = schedule.weeklyHours.map((x, i) => `Week ${i + 1}: ${x} hours`).join("<br>");
}

function hidePreview() {
    document.getElementById("preview").style.display = "none";
}

async function sendCalculation(data, preview) {
    const headers = {
      'Accept': "application/json",
      'Content-Type': "application/json"
    };
    const request = new Request(`api/schedules${preview ? "/preview" : ""}`,
        { method: "POST", body: JSON.stringify(data), headers });
    const response = await fetch(request);
    const result = {};
    try {
        if (response.ok) {
            const saved = await response.json();
            result.data = saved;
        } else {
            const contentType = response.headers.get("content-type");
            const isJson = contentType.includes("application/problem+json") || contentType.includes("application/json")
            const error = isJson ? (await response.json()) : `Error. Status: ${response.status}"`;
            result.error = error;
        }
    } catch (e) {
        result.error = e;
    }
    return result;
}

async function calculateForm(form, preview) {
    const request = prepareRequest(new FormData(form));
    const result = await sendCalculation(request, preview);
    if (result.error) {
        showError(result.error);
    } else {
        hideError();
    }
    return result;
}

async function submitForm(e) {
    e.preventDefault();
    hidePreview();
    await calculateForm(e.target, false);
    loadSchedules();
}

async function preview(e) {
    e.preventDefault();
    hidePreview();
    const result = await calculateForm(e.target.form, true);
    if (result.data) {
        showPreview(result.data);
    }
}

function formatDate(d) {
    return new Date(d).toISOString().split("T")[0];
}

loadCourses();
loadCalculationStrategies();

const startDate = new Date();
startDate.setHours(12, 0, 0, 0);
const endDate = new Date(startDate);
endDate.setDate(endDate.getDate() + 14);

document.getElementById("studentId").value = 1;
document.getElementById("startDate").value = formatDate(startDate);
document.getElementById("endDate").value = formatDate(endDate);
hideError();
hidePreview();
loadSchedules();
