# Tree: TreeMenu

## Usage

```js
import { TreeMenu } from 'asc-web-components';
```

#### Description

Tree menu description

#### Usage

```js
<TreeMenu data={data}/>;
```

#### Properties

| Props                  | Type     | Required | Values                       | Default | Description                                                                                            |
| ---------------------- | -------- | :------: | ---------------------------- | ------- | ------------------------------------------------------------------------------------------------------ |
| `checkable`            | `bool`   |    -     | -                            | `false` | Whether support checked                                                                                |
| `draggable`            | `bool`   |    -     | -                            | `false` | Whether can drag treeNode                                                                              |
| `disabled`             | `bool`   |    -     | -                            | `false` | Whether disabled the tree                                                                              |
| `multiple`             | `bool`   |    -     | -                            | `false` | Whether multiple select                                                                                |
| `showIcon`             | `bool`   |    -     | -                            | `true`  | Whether show icon                                                                                      |
| `showLine`             | `bool`   |    -     | -                            | `false` | Whether show line                                                                                      |
| `autoExpandParent`     | `bool`   |    -     | -                            | `false` | Whether auto expand parent treeNodes                                                                   |
| `defaultExpandAll`     | `bool`   |    -     | -                            | `false` | Expand all treeNodes                                                                                   |
| `defaultExpandParent`  | `bool`   |    -     | -                            | `true`  | Auto expand parent treeNodes when init                                                                 |
| `onExpand`             | `func`   |    -     | -                            |    -    | fire on treeNode expand or not                                                                         |
| `onDragEnd`            | `func`   |    -     | -                            |    -    | it execs when fire the tree's dragend event                                                            |
| `onDragEnter`          | `func`   |    -     | -                            |    -    | it execs when fire the tree's dragenter event                                                          |
| `onDragLeave`          | `func`   |    -     | -                            |    -    | it execs when fire the tree's dragleave event                                                          |
| `onDragOver`           | `func`   |    -     | -                            |    -    | it execs when fire the tree's dragover event                                                           |
| `onDragStart`          | `func`   |    -     | -                            |    -    | it execs when fire the tree's dragstart event                                                          |
| `onDrop`               | `func`   |    -     | -                            |    -    | it execs when fire the tree's drop event                                                               |
| `onLoad`               | `func`   |    -     | -                            |    -    | Trigger when a node is loaded. If you set the loadedKeys, you must handle onLoad to avoid infinity loop|
| `onMouseEnter`         | `func`   |    -     | -                            |    -    | call when mouse enter a treeNode                                                                       |
| `onMouseLeave`         | `func`   |    -     | -                            |    -    | call when mouse leave a treeNode                                                                       |
| `onRightClick`         | `func`   |    -     | -                            |    -    | select current treeNode and show customized contextmenu                                                |
| `onSelect`             | `func`   |    -     | -                            |    -    | click the treeNode to fire                                                                             |




### Main Functions and use cases are:
