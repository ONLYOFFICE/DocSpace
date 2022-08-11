# TabContainer

Custom Tabs menu

### Usage

```js
import TabContainer from "@docspace/components/tabs-container";
```

```js
const array_items = [
  {
    key: "0",
    title: "Title1",
    content: (
      <div>
        <div>
          <button>BUTTON</button>
        </div>
        <div>
          <button>BUTTON</button>
        </div>
        <div>
          <button>BUTTON</button>
        </div>
      </div>
    ),
  },
  {
    key: "1",
    title: "Title2",
    content: (
      <div>
        <div>
          <label>LABEL</label>
        </div>
        <div>
          <label>LABEL</label>
        </div>
        <div>
          <label>LABEL</label>
        </div>
      </div>
    ),
  },
  {
    key: "2",
    title: "Title3",
    content: (
      <div>
        <div>
          <input></input>
        </div>
        <div>
          <input></input>
        </div>
        <div>
          <input></input>
        </div>
      </div>
    ),
  },
];
```

```jsx
<TabContainer>{array_items}</TabContainer>
```

### TabContainer Properties

| Props          |   Type    | Required | Values | Default | Description                        |
| -------------- | :-------: | :------: | :----: | :-----: | ---------------------------------- |
| `isDisabled`   | `boolean` |    -     |   -    |    -    | Disable the TabContainer           |
| `selectedItem` | `number`  |    -     |   -    |   `0`   | Selected title of tabs container   |
| `onSelect`     |  `func`   |    -     |   -    |    -    | Triggered when a title is selected |

### Array Items Properties

| Props     |   Type   | Required | Values | Default | Description           |
| --------- | :------: | :------: | :----: | :-----: | --------------------- |
| `id`      | `string` |    ✅    |   -    |    -    | Index of object array |
| `title`   | `string` |    ✅    |   -    |    -    | Tabs title            |
| `content` | `object` |    ✅    |   -    |    -    | Content in Tab        |
