# Submenu

### Usage

```js
import Submenu from "@docspace/components/submenu";
```

```jsx
<Submenu
  data={[
    {
      id: "FileInput",
      name: "File Input",
      content: (
        <FileInput
          accept=".doc, .docx"
          id="file-input-id"
          name="demoFileInputName"
          onInput={function noRefCheck() {}}
          placeholder="Input file"
        />
      ),
    },
    {
      id: "ToggleButton",
      name: "Toggle Button",
      content: (
        <ToggleButton
          className="toggle className"
          id="toggle id"
          label="label text"
          onChange={() => {}}
        />
      ),
    },
  ]}
  startSelect={1}
/>
```

#### Data is an array of objects with following fields:

- id - unique id
- name - header in submenu
- content - HTML object that will be rendered under submenu

##### Example:

```jsx
{
  id: "FileInput",
  name: "File Input",
  content: (
    <FileInput
      accept=".doc, .docx"
      id="file-input-id"
      name="demoFileInputName"
      onInput={function noRefCheck() {}}
      placeholder="Input file"
    />
  ),
},
```

### Properties

| Props         |      Type       | Required | Values | Default | Description                                                                   |
| ------------- | :-------------: | :------: | :----: | :-----: | ----------------------------------------------------------------------------- |
| `data`        |     `array`     |    ✅    |   -    |    -    | List of elements                                                              |
| `startSelect` | `obj`, `number` |    -     |   -    |    0    | Object from data that will be chosen first **OR** Its index in **data** array |
