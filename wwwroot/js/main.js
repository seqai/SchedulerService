async function loadCourses() {
    const request = new Request('api/courses');
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

loadCourses();


