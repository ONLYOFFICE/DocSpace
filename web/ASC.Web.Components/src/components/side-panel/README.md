# SidePanel

#### Description

SidePanel is used for displaying side panels

#### Usage

```js
import { SidePanel } from 'asc-web-components';

<SidePanel visible={false} scale={false}/>
```

#### Properties

| Props           | Type                      | Required | Values | Default | Description                                      |
| --------------- | ------------------------- | :------: | -------| ------- | ------------------------------------------------ |
| `visible`       | `bool`                    |          |        |         | Display side panels or not                       |
| `scale`         | `bool`                    |          |        |         | Indicates the side panel has scale               |
| `headerContent` | `string/element/elements` |          |        |         | Header content                                   |
| `bodyContent`   | `string/element/elements` |          |        |         | Body content                                     |
| `footerContent` | `string/element/elements` |          |        |         | Footer content                                   |
| `onClose`       | `func`                    |          |        |         | Will be triggered when a close button is clicked |