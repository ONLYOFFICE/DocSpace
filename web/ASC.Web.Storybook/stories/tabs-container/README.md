# TabContainer

#### Description

Custom Tabs menu

#### Usage

```js
import { TabContainer } from 'asc-web-components';

const array_items = [
    {
        id: "0",
        title: <label> Title1 </label>,
        content:
            <div >
                <div> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> </div>
                <div> <button>BUTTON</button> </div>
            </div>
    },
    {
        id: "1",
        title: <label> Title2 </label>,
        content:
            <div >
                <div> <label>LABEL</label> </div>
                <div> <label>LABEL</label> </div>
                <div> <label>LABEL</label> </div>
            </div>
    },
    {
        id: "2",
        title: <label> Title3 </label>,
        content:
            <div>
                <div> <input></input> </div>
                <div> <input></input> </div>
                <div> <input></input> </div>
            </div>
    }
];

<TabContainer>{array_items}</TabContainer>
```

#### Array Items Properties

| Props      | Type        | Required | Values                                    | Default      | Description           |
| ---------- | ----------- | :------: | ----------------------------------------- | ------------ | --------------------- |
| `id`                | `string` |    true    | -                            | -       | Index of object array 
| `title`                | `string` |    true    | -                            | -       | Tabs title         
| `content`                | `object` |    true   | -                            | -       | Content in Tab


#### TabContainer Properties

| Props      | Type        | Required | Values                                    | Default      | Description           |
| ---------- | ----------- | :------: | ----------------------------------------- | ------------ | --------------------- |
| `isDisabled`                | `boolean` |    -    | -                            | -       | Disable the TabContainer


