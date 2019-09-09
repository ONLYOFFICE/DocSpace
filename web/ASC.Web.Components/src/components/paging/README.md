# Paging

#### Description

Paging is used to navigate med content pages.

#### Usage

```js
import { Paging } from 'asc-web-components';

<Paging 
    previousLabel='Previous'
    nextLabel='Next'
    selectedPageItem={{ label: "1 of 1"}}
    selectedCountItem={{ label: "25 per page"}}
    previousAction={() => console.log('Prev')}
    nextAction={() => console.log('Next')}
    openDirection='bottom'
    onSelectPage={(a) => console.log(a)}
    onSelectCount={(a) => console.log(a)}
/>

```

#### Properties

| Props                  | Type              | Required | Values                       | Default | Description                                  |
| ---------------------- | ----------------- | :------: | ---------------------------- | ------- | -------------------------------------------- |
| `pageItems`            | `array`           |    -     | -                            | -       | Paging combo box items                       |
| `countItems`           | `array`           |    -     | -                            | -       | Items per page combo box items               |
| `previousLabel`        | `string`          |    -     | -                            | `Previous`| Label for previous button                  |
| `nextLabel`            | `string`          |    -     | -                            | `Next`  | Label for next button                        |
| `previousAction`       | `function`        |    -     | -                            | -       | Action for previous button                   |
| `nextAction`           | `function`        |    -     | -                            | -       | Action for next button                       |
| `openDirection`        | `string`          |    -     | `top`, `bottom`              | `bottom`| Indicates opening direction of combo box     |
| `disablePrevious`      | `bool`            |    -     | -                            | `false` | Set previous button disabled                 |
| `disableNext`          | `bool`            |    -     | -                            | `false` | Set next button disabled                     |
| `selectedPageItem`     | `object`          |    -     | -                            | -       | Initial value for pageItems                  |
| `selectedCountItem`    | `object`          |    -     | -                            | -       | Initial value for countItems                 |