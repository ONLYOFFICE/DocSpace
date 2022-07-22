# Paging

Paging is used to navigate med content pages

### Usage

```js
import Paging from "@docspace/components/paging";
```

```jsx
<Paging
  previousLabel="Previous"
  nextLabel="Next"
  selectedPageItem={{ label: "1 of 1" }}
  selectedCountItem={{ label: "25 per page" }}
  previousAction={() => console.log("Prev")}
  nextAction={() => console.log("Next")}
  openDirection="bottom"
  onSelectPage={(a) => console.log(a)}
  onSelectCount={(a) => console.log(a)}
/>
```

### Properties

| Props               |      Type      | Required |     Values      |  Default   | Description                              |
| ------------------- | :------------: | :------: | :-------------: | :--------: | ---------------------------------------- |
| `className`         |    `string`    |    -     |        -        |     -      | Accepts class                            |
| `countItems`        |    `array`     |    -     |        -        |     -      | Items per page combo box items           |
| `disableNext`       |     `bool`     |    -     |        -        |  `false`   | Set next button disabled                 |
| `disablePrevious`   |     `bool`     |    -     |        -        |  `false`   | Set previous button disabled             |
| `id`                |    `string`    |    -     |        -        |     -      | Accepts id                               |
| `nextAction`        |   `function`   |    -     |        -        |     -      | Action for next button                   |
| `nextLabel`         |    `string`    |    -     |        -        |   `Next`   | Label for next button                    |
| `openDirection`     |    `string`    |    -     | `top`, `bottom` |  `bottom`  | Indicates opening direction of combo box |
| `pageItems`         |    `array`     |    -     |        -        |     -      | Paging combo box items                   |
| `previousAction`    |   `function`   |    -     |        -        |     -      | Action for previous button               |
| `previousLabel`     |    `string`    |    -     |        -        | `Previous` | Label for previous button                |
| `selectedCountItem` |    `object`    |    -     |        -        |     -      | Initial value for countItems             |
| `selectedPageItem`  |    `object`    |    -     |        -        |     -      | Initial value for pageItems              |
| `style`             | `obj`, `array` |    -     |        -        |     -      | Accepts css style                        |
