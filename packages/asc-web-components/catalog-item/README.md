# CatalogItem

Is a item of CatalogBloc component

### Usage

```js
import CatalogItem from "@appserver/components/catalog-item";
```

```jsx
<CatalogItem />
```

Display catalog item. On tablet can show only icon. If is it end of block - adding margin bottom.

### Properties

| Props          |      Type      | Required | Values | Default | Description                                                                   |
| -------------- | :------------: | :------: | :----: | :-----: | ----------------------------------------------------------------------------- |
| `className`    |    `string`    |    -     |   -    |    -    | Accepts class                                                                 |
| `id`           |    `string`    |    -     |   -    |    -    | Accepts id                                                                    |
| `style`        | `obj`, `array` |    -     |   -    |    -    | Accepts css style                                                             |
| `icon`         |    `string`    |    -     |   -    |    -    | Catalog item icon                                                             |
| `text`         |    `string`    |    -     |   -    |    -    | Catalog item text                                                             |
| `showText`     |     `bool`     |    -     |   -    | `false` | Tells when the catalog item should display text                               |
| `onClick`      |     `func`     |    -     |   -    |    -    | What the catalog item will trigger when clicked                               |
| `showInitial`  |     `bool`     |    -     |   -    | `false` | Tells when the catalog item should display initial text(first symbol of text) |
| `isEndOfBlock` |     `bool`     |    -     |   -    | `false` | Tells when the catalog item should be end of block (adding margin-bottom)     |
| `showBadge`    |     `bool`     |    -     |   -    | `false` | Tells when the catalog item should display badge                              |
| `labelBadge`   |    `string`    |    -     |   -    |    -    | Label for badge                                                               |
| `iconBadge`    |    `string`    |    -     |   -    |    -    | Icon for badge                                                                |
| `onClickBadge` |     `func`     |    -     |   -    |    -    | What the catalog item badge will trigger when clicked                         |
