# Paging

#### Description

Paging is used to navigate med content pages.

#### Usage

```js
import { Paging } from 'asc-web-components';

<ComboBox previousLabel='Previous' nextLabel='Next' pageItems={pageItems} perPageItems={perPageItems}/>
```

#### Properties

| Props                  | Type              | Required | Values                       | Default | Description                                  |
| ---------------------- | ----------------- | :------: | ---------------------------- | ------- | -------------------------------------------- |
| `pageItems`            | `object`          |     -    | -                            | -       | Paging combo box items                       |
| `perPageItems`         | `object`          |    ✅    | -                            | -       | Items per page combo box items               |
| `previousLabel`        | `string`          |    -     | -                            | `Previous`| Label for previous button                    |
| `nextLabel`            | `string`          |    -     | -                            | `Next`  | Label for next button                        |
| `previousAction`       | `function`        |    -     | -                            | -       | Action for previous button                   |
| `nextAction`           | `function`        |    -     | -                            | -       | Action for next button                       |