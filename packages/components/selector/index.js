import React from "react";
import PropTypes from "prop-types";

import Header from "./sub-components/Header";
import Body from "./sub-components/Body";
import Footer from "./sub-components/Footer";

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
  searchEmptyScreenImage,
  searchEmptyScreenHeader,
  searchEmptyScreenDescription,
  hasNextPage,
  isNextPageLoading,
  loadNextPage,
  totalItems,
  isLoading,
  searchLoader,
  rowLoader,
}) => {
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

      if (item.isSelected) {
        setNewSelectedItems([]);
        compareSelectedItems([]);
      } else {
        setNewSelectedItems([newItem]);
        compareSelectedItems([newItem]);
      }
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

      if (newList.length === 0 && selectedItems.length === 0) {
        return setFooterVisible(false);
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
      !isNextPageLoading && loadNextPage && loadNextPage(startIndex);
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
        searchEmptyScreenImage={searchEmptyScreenImage}
        searchEmptyScreenHeader={searchEmptyScreenHeader}
        searchEmptyScreenDescription={searchEmptyScreenDescription}
        hasNextPage={hasNextPage}
        isNextPageLoading={isNextPageLoading}
        loadMoreItems={loadMoreItems}
        totalItems={totalItems}
        isLoading={isLoading}
        searchLoader={searchLoader}
        rowLoader={rowLoader}
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
  /** Accepts id  */
  id: PropTypes.string,
  /** Accepts class  */
  className: PropTypes.oneOfType([PropTypes.string, PropTypes.array]),
  /** Accepts css style */
  style: PropTypes.object,

  /** Selector header text */
  headerLabel: PropTypes.string,
  /** What the header arrow will trigger when clicked */
  onBackClick: PropTypes.func,

  /** Placeholder for search input */
  searchPlaceholder: PropTypes.string,
  /** Start value for search input */
  searchValue: PropTypes.string,
  /** What the search input will trigger when user stopped typing */
  onSearch: PropTypes.func,
  /** What the clear icon of search input will trigger when clicked */
  onClearSearch: PropTypes.func,

  /** Displaying items */
  items: PropTypes.array,
  /** What the item will trigger when clicked */
  onSelect: PropTypes.func,

  /** Allows you to select multiple objects */
  isMultiSelect: PropTypes.bool,
  /** Tells when the items should present a checked state */
  selectedItems: PropTypes.array,
  /** Accept button text */
  acceptButtonLabel: PropTypes.string,
  /** What the accept button will trigger when clicked */
  onAccept: PropTypes.func,

  /** Add option for select all items */
  withSelectAll: PropTypes.bool,
  /** Text for select all component */
  selectAllLabel: PropTypes.string,
  /** Icon for select all component */
  selectAllIcon: PropTypes.string,
  /** What the select all will trigger when clicked */
  onSelectAll: PropTypes.func,

  /** Add combobox for displaying access rights at footer */
  withAccessRights: PropTypes.bool,
  /** Access rights items */
  accessRights: PropTypes.array,
  /** Selected access rights items */
  selectedAccessRight: PropTypes.object,
  /** What the access right will trigger when changed */
  onAccessRightsChange: PropTypes.func,

  /** Add cancel button at footer */
  withCancelButton: PropTypes.bool,
  /** Displaying text at cancel button */
  cancelButtonLabel: PropTypes.string,
  /** What the cancel button will trigger when clicked */
  onCancel: PropTypes.func,

  /** Image for default empty screen */
  emptyScreenImage: PropTypes.string,
  /** Header for default empty screen */
  emptyScreenHeader: PropTypes.string,
  /** Description for default empty screen */
  emptyScreenDescription: PropTypes.string,

  /** Image for search empty screen */
  searchEmptyScreenImage: PropTypes.string,
  /** Header for search empty screen */
  searchEmptyScreenHeader: PropTypes.string,
  /** Description for search empty screen */
  searchEmptyScreenDescription: PropTypes.string,

  /** Count items for infinity scroll */
  totalItems: PropTypes.number,
  /** Has next page for infinity scroll */
  hasNextPage: PropTypes.bool,
  /** Tells when next page already loading */
  isNextPageLoading: PropTypes.bool,
  /** Function for load next page */
  loadNextPage: PropTypes.func,

  /** Set loading state for select */
  isLoading: PropTypes.bool,
  /** Loader element for search block */
  searchLoader: PropTypes.node,
  /** Loader element for item */
  rowLoader: PropTypes.node,
};

Selector.defaultProps = {
  isMultiSelect: false,
  withSelectAll: false,
  withAccessRights: false,
  withCancelButton: false,

  selectedItems: [],
};

export default Selector;
