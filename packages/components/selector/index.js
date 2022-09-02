import React from "react";
import PropTypes from "prop-types";

import Header from "./sub-components/header";
import Body from "./sub-components/body";
import Footer from "./sub-components/footer";

import { StyledSelector } from "./StyledSelector";

const Selector = ({
  id,
  className,
  style,
  headerLabel,
  onBackClick,
  searchPlaceholder,
  searchValue,
  onSearch,
  onClearSearch,
  items,
  onSelect,
  isMultiSelect,
  selectedItems,
  acceptButtonLabel,
  onAccept,
  withSelectAll,
  selectAllLabel,
  selectAllIcon,
  onSelectAll,
  withAccessRights,
  accessRights,
  selectedAccessRight,
  onAccessRightsChange,
  withCancelButton,
  cancelButtonLabel,
  onCancel,
  emptyScreenImage,
  emptyScreenHeader,
  emptyScreenDescription,
  hasNextPage,
  isNextPageLoading,
  loadNextPage,
  totalItems,
}) => {
  const [isLoading, setIsLoading] = React.useState(false);

  const [footerVisible, setFooterVisible] = React.useState(false);

  const [isSearch, setIsSearch] = React.useState(false);

  const [renderedItems, setRenderedItems] = React.useState([]);
  const [newSelectedItems, setNewSelectedItems] = React.useState([]);

  const [selectedAccess, setSelectedAccess] = React.useState({});

  const onBackClickAction = React.useCallback(() => {
    onBackClick && onBackClick();
  }, [onBackClick]);

  const onSearchAction = React.useCallback(
    (value) => {
      onSearch && onSearch(value);
      if (value) {
        setIsSearch(true);
      } else {
        setIsSearch(false);
      }
    },
    [onSearch]
  );

  const onClearSearchAction = React.useCallback(() => {
    onClearSearch && onClearSearch();
    setIsSearch(false);
  }, [onClearSearch]);

  const onSelectAction = (item) => {
    onSelect &&
      onSelect({
        id: item.id,
        avatar: item.avatar,
        icon: item.icon,
        label: item.label,
      });

    if (isMultiSelect) {
      setRenderedItems((value) => {
        const idx = value.findIndex((x) => item.id === x.id);

        const newValue = value.map((item) => ({ ...item }));

        if (idx === -1) return newValue;

        newValue[idx].isSelected = !value[idx].isSelected;

        return newValue;
      });

      if (item.isSelected) {
        setNewSelectedItems((value) => {
          const newValue = value
            .filter((x) => x.id !== item.id)
            .map((x) => ({ ...x }));
          compareSelectedItems(newValue);
          return newValue;
        });
      } else {
        setNewSelectedItems((value) => {
          value.push({
            id: item.id,
            avatar: item.avatar,
            icon: item.icon,
            label: item.label,
          });

          compareSelectedItems(value);

          return value;
        });
      }
    } else {
      setRenderedItems((value) => {
        const idx = value.findIndex((x) => item.id === x.id);

        const newValue = value.map((item) => ({ ...item, isSelected: false }));

        if (idx === -1) return newValue;

        newValue[idx].isSelected = !item.isSelected;

        return newValue;
      });

      const newItem = {
        id: item.id,
        avatar: item.avatar,
        icon: item.icon,
        label: item.label,
      };

      setNewSelectedItems([newItem]);
      compareSelectedItems([newItem]);
    }
  };

  const onSelectAllAction = React.useCallback(() => {
    onSelectAll && onSelectAll();
    if (newSelectedItems.length === 0) {
      const cloneItems = items.map((x) => ({ ...x }));

      const cloneRenderedItems = items.map((x) => ({ ...x, isSelected: true }));

      setRenderedItems(cloneRenderedItems);
      setNewSelectedItems(cloneItems);
      compareSelectedItems(cloneItems);
    } else {
      const cloneRenderedItems = items.map((x) => ({
        ...x,
        isSelected: false,
      }));

      setRenderedItems(cloneRenderedItems);
      setNewSelectedItems([]);
      compareSelectedItems([]);
    }
  }, [items, newSelectedItems]);

  const onAcceptAction = React.useCallback(() => {
    onAccept && onAccept(newSelectedItems, selectedAccess);
  }, [newSelectedItems, selectedAccess]);

  const onCancelAction = React.useCallback(() => {
    onCancel && onCancel();
  }, [onCancel]);

  const onChangeAccessRightsAction = React.useCallback(
    (access) => {
      setSelectedAccess({ ...access });
      onAccessRightsChange && onAccessRightsChange(access);
    },
    [onAccessRightsChange]
  );

  const compareSelectedItems = React.useCallback(
    (newList) => {
      let isEqual = true;

      if (selectedItems.length !== newList.length) {
        return setFooterVisible(true);
      }

      newList.forEach((item) => {
        isEqual = selectedItems.some((x) => x.id === item.id);
      });

      return setFooterVisible(!isEqual);
    },
    [selectedItems]
  );

  const loadMoreItems = React.useCallback(
    (startIndex) => {
      loadNextPage && loadNextPage(startIndex);
    },
    [isNextPageLoading, loadNextPage]
  );

  React.useEffect(() => {
    setSelectedAccess({ ...selectedAccessRight });
  }, [selectedAccessRight]);

  React.useEffect(() => {
    if (items && selectedItems) {
      if (selectedItems.length === 0 || !isMultiSelect) {
        const cloneItems = items.map((x) => ({ ...x, isSelected: false }));
        return setRenderedItems(cloneItems);
      }

      const newItems = items.map((item) => {
        const { id } = item;

        const isSelected = selectedItems.some(
          (selectedItem) => selectedItem.id === id
        );

        return { ...item, isSelected };
      });

      const cloneSelectedItems = selectedItems.map((item) => ({ ...item }));

      setRenderedItems(newItems);
      setNewSelectedItems(cloneSelectedItems);
      compareSelectedItems(cloneSelectedItems);
    }
  }, [items, selectedItems, isMultiSelect, compareSelectedItems]);

  return (
    <StyledSelector id={id} className={className} style={style}>
      <Header onBackClickAction={onBackClickAction} headerLabel={headerLabel} />

      <Body
        footerVisible={footerVisible}
        isSearch={isSearch}
        isAllIndeterminate={
          newSelectedItems.length !== renderedItems.length &&
          newSelectedItems.length !== 0
        }
        isAllChecked={newSelectedItems.length === renderedItems.length}
        placeholder={searchPlaceholder}
        value={searchValue}
        onSearch={onSearchAction}
        onClearSearch={onClearSearchAction}
        items={renderedItems}
        isMultiSelect={isMultiSelect}
        onSelect={onSelectAction}
        withSelectAll={withSelectAll}
        selectAllLabel={selectAllLabel}
        selectAllIcon={selectAllIcon}
        onSelectAll={onSelectAllAction}
        emptyScreenImage={emptyScreenImage}
        emptyScreenHeader={emptyScreenHeader}
        emptyScreenDescription={emptyScreenDescription}
        hasNextPage={hasNextPage}
        loadMoreItems={loadMoreItems}
        totalItems={totalItems}
      />

      {footerVisible && (
        <Footer
          isMultiSelect={isMultiSelect}
          acceptButtonLabel={acceptButtonLabel}
          selectedItemsCount={newSelectedItems.length}
          withCancelButton={withCancelButton}
          cancelButtonLabel={cancelButtonLabel}
          withAccessRights={withAccessRights}
          accessRights={accessRights}
          selectedAccessRight={selectedAccess}
          onAccept={onAcceptAction}
          onCancel={onCancelAction}
          onChangeAccessRights={onChangeAccessRightsAction}
        />
      )}
    </StyledSelector>
  );
};

Selector.propTypes = {
  id: PropTypes.string,
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  style: PropTypes.object,

  headerLabel: PropTypes.string,
  onBackClick: PropTypes.func,

  searchPlaceholder: PropTypes.string,
  searchValue: PropTypes.string,
  onSearch: PropTypes.func,
  onClearSearch: PropTypes.func,

  items: PropTypes.array,
  onSelect: PropTypes.func,

  isMultiSelect: PropTypes.bool,
  selectedItems: PropTypes.array,
  acceptButtonLabel: PropTypes.string,
  onAccept: PropTypes.func,

  withSelectAll: PropTypes.bool,
  selectAllLabel: PropTypes.string,
  selectAllIcon: PropTypes.string,
  onSelectAll: PropTypes.func,

  withAccessRights: PropTypes.bool,
  accessRights: PropTypes.array,
  selectedAccessRight: PropTypes.object,
  onAccessRightsChange: PropTypes.func,

  withCancelButton: PropTypes.bool,
  cancelButtonLabel: PropTypes.string,
  onCancel: PropTypes.func,

  emptyScreenImage: PropTypes.string,
  emptyScreenHeader: PropTypes.string,
  emptyScreenDescription: PropTypes.string,

  totalItems: PropTypes.number,
  hasNextPage: PropTypes.bool,
  isNextPageLoading: PropTypes.bool,
  loadNextPage: PropTypes.func,
};

Selector.defaultProps = {
  isMultiSelect: false,
  withSelectAll: false,
  withAccessRights: false,
  withCancelButton: false,

  selectedItems: [],
};

export default Selector;
