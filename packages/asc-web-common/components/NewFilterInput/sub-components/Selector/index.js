import React from 'react';

import { getUserList, getSelectorUserList } from '../../../../api/people';
import { getGroupList } from '../../../../api/groups';
import Filter from '../../../../api/people/filter';

import Header from './Header';
import Search from './Search';
import Content from './Content';
import Footer from './Footer';

import { StyledSelector } from './StyledSelector';

const Selector = ({
  headerLabel,
  selectedItems,
  isAuthor,
  groupType,
  changeFilterValue,
  hideSelector,
  props,
}) => {
  const [header, setHeader] = React.useState();
  const [isLoading, setIsLoading] = React.useState(false);

  const [itemsList, setItemsList] = React.useState([]);
  const [groupsList, setGroupsList] = React.useState([]);

  const [listHeader, setListHeader] = React.useState(null);

  const [selectedItemsList, setSelectedItemsList] = React.useState([]);

  const getUsers = React.useCallback(async (searchValue, groupId) => {
    const filter = Filter.getDefault();

    if (searchValue) filter.search = searchValue;
    if (groupId) filter.group = groupId;

    const usersData = [];

    if (searchValue || (groupId && groupId !== 'all')) {
      const data = await getUserList(filter);

      usersData.push(...data.items);
    } else {
      const data = await getSelectorUserList();

      usersData.push(...data.items);
    }

    const users = [];

    usersData.forEach((user) => {
      const newUser = {
        name: user.displayName,
        id: user.id,
        avatar: user.avatar,
        isGroup: false,
      };

      newUser.groups = user.groups ? [...user.groups, { id: 'all' }] : [{ id: 'all' }];

      users.push({ ...newUser });
    });

    return users;
  }, []);

  const getGroups = React.useCallback(async (value) => {
    const groupsData = value ? await getGroupList(false, value) : await getGroupList();

    const groups = [];

    groupsData.forEach((group) => {
      const newGroup = {
        name: group.name,
        id: group.id,
        avatar: '',
        isGroup: isAuthor,
      };

      if (!isAuthor) {
        newGroup.groups = [{ id: 'all' }];
        newGroup.isSelected = false;
      } else {
        newGroup.itemsCount = 0;
        newGroup.selectedItemsCount = 0;
      }

      groups.push({ ...newGroup });
    });

    return groups;
  }, []);

  const getData = async () => {
    setIsLoading(true);

    const groupsData = await getGroups();

    const groups = [
      {
        name: 'All users',
        id: 'all',
        avatar: '/static/images/departments.group.react.svg',
        isGroup: true,
        itemsCount: 0,
        selectedItemsCount: 0,
      },
    ];

    if (!isAuthor) {
      groupsData.forEach((group) => {
        group.groups = groups;
      });

      setListHeader({ ...groups[0] });
      setGroupsList([...groups]);
      setItemsList([...groupsData]);
    } else {
      groups.push(...groupsData);

      const users = await getUsers();

      users.forEach((user) => {
        if (selectedItems.length > 0) {
          user.isSelected = !!selectedItems.find((item) => item.id === user.id);
        }

        user.groups.forEach((userGroup) => {
          const idx = groups.findIndex((group) => group.id === userGroup.id);

          groups[idx].itemsCount++;

          if (user.isSelected) {
            groups[idx].selectedItemsCount++;
          }
        });
      });

      setGroupsList([...groups]);
      setItemsList([...groups]);
    }

    setIsLoading(false);
  };

  const showGroupItems = React.useCallback(
    async (id) => {
      if (isAuthor) {
        const group = groupsList.find((item) => item.id === id);

        setListHeader({ ...group });
        setHeader(group.name);

        setIsLoading(true);

        const users = await getUsers(null, id);

        users.forEach((user) => {
          if (selectedItemsList.length > 0) {
            user.isSelected = !!selectedItemsList.find((item) => item.id === user.id);
          }
        });

        setItemsList([...users]);

        setIsLoading(false);
      }
    },
    [groupsList, selectedItemsList],
  );

  const selectGroupItems = React.useCallback(
    (id) => {
      itemsList.forEach((item) => {
        if (item.groups.findIndex((group) => group.id === id)) {
          selectItem(item.id);
        }
      });
    },
    [itemsList, selectItem],
  );

  const selectItem = React.useCallback(
    async (id) => {
      const selectedIdx = selectedItemsList.findIndex((item) => item.id === id);

      if (selectedIdx !== -1) {
        const selectedItems = selectedItemsList.filter((item, idx) => idx !== selectedIdx);

        setSelectedItemsList([...selectedItems]);
      } else {
        const selectedItems = [...selectedItemsList, { id: id }];

        setSelectedItemsList([...selectedItems]);
      }

      const items = [...itemsList];
      const groups = [...groupsList];

      const idx = items.findIndex((item) => item.id === id);

      items[idx].isSelected = selectedIdx === -1;

      setItemsList([...items]);

      items[idx].groups.forEach((itemGroup) => {
        if (selectedIdx === -1) {
          groups.find((group) => group.id === itemGroup.id).selectedItemsCount++;
        } else {
          groups.find((group) => group.id === itemGroup.id).selectedItemsCount--;
        }
      });

      setGroupsList([...groups]);

      const newListHeader = { ...listHeader };

      if (selectedIdx === -1) {
        newListHeader.selectedItemsCount++;
      } else {
        newListHeader.selectedItemsCount--;
      }

      setListHeader({ ...newListHeader });
    },
    [itemsList, groupsList, selectedItemsList, listHeader],
  );

  const onClickItemAction = React.useCallback(
    (isGroup, id, isHeader) => {
      if (isGroup) {
        if (!isHeader) {
          showGroupItems(id);
        } else {
          selectGroupItems(id);
        }
      } else {
        selectItem(id);
      }
    },
    [showGroupItems, selectGroupItems, selectItem],
  );

  const onClickBackAction = React.useCallback(async () => {
    if (listHeader && isAuthor) {
      setHeader(headerLabel);
      setListHeader(null);
      setItemsList([...groupsList]);
    } else {
      hideSelector();
    }
  }, [listHeader, groupsList, isAuthor, hideSelector]);

  const onSearch = React.useCallback(
    async (value) => {
      setIsLoading(true);
      if (value) {
        setListHeader(null);

        const data = isAuthor ? await getUsers(value) : await getGroups(value);

        setItemsList([...data]);
      } else {
        if (!isAuthor) {
          setListHeader({ ...groupsList[0] });
        }

        const data = isAuthor ? await getUsers() : await getGroups();

        setItemsList([...data]);
      }
      setIsLoading(false);
    },
    [groupsList[0], isAuthor, getUsers, getGroups],
  );

  const onClickAddAction = React.useCallback(() => {
    changeFilterValue && changeFilterValue(groupType, selectedItemsList, false);
    hideSelector();
  }, [groupType, selectedItems, selectedItemsList, changeFilterValue, hideSelector]);

  React.useEffect(() => {
    setSelectedItemsList([...selectedItems]);
  }, [selectedItems]);

  React.useEffect(() => {
    setHeader(headerLabel);
  }, [headerLabel]);

  React.useEffect(() => {
    getData();
  }, []);

  return (
    <StyledSelector>
      <Header label={header} onClick={onClickBackAction} />
      <Search placeholder={'Search users'} onSearch={onSearch} />
      <Content
        itemsList={itemsList}
        listHeader={listHeader}
        isLoading={isLoading}
        onClickItem={onClickItemAction}
      />
      <Footer count={selectedItemsList.length} onClick={onClickAddAction} />
    </StyledSelector>
  );
};

export default React.memo(Selector);
