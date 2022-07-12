import React from "react";
import styled, { css } from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";
import { isMobile } from "react-device-detect";

import { tablet } from "@appserver/components/utils/device";

import Navigation from "@appserver/common/components/Navigation";
import Loaders from "@appserver/common/components/Loaders";

import TableGroupMenu from "@appserver/components/table-container/TableGroupMenu";

import withLoader from "../../../../HOCs/withLoader";
import { Consumer } from "@appserver/components/utils/context";
import { FolderType } from "@appserver/common/constants";
import DropDownItem from "@appserver/components/drop-down-item";

const StyledContainer = styled.div`
  .table-container_group-menu {
    ${(props) =>
      props.viewAs === "table"
        ? css`
            margin: 0px -20px;
            width: calc(100% + 40px);
          `
        : css`
            margin: 0px -20px;
            width: calc(100% + 40px);
          `}

    @media ${tablet} {
      margin: 0 -16px;
      width: calc(100% + 32px);
    }

    ${isMobile &&
    css`
      margin: 0 -16px;
      width: calc(100% + 32px);
    `}
  }
`;

const SectionHeaderContent = ({
  t,

  createRoom,
  viewAs,
  title,
  showText,
  isArchive,
  isDesktop,
  isTabletView,
  tReady,
  navigationPath,

  isHeaderVisible,
  isHeaderChecked,
  isHeaderIndeterminate,
  checkboxMenuItems,
  getRoomCheckboxTitle,
  setSelected,

  getHeaderMenu,

  toggleInfoPanel,
  isInfoPanelVisible,
}) => {
  const getContextOptionsPlus = () => {
    return [];
  };

  const getContextOptionsFolder = () => {
    return [];
  };

  const renderGroupMenu = React.useCallback(() => {
    const onSelected = (e) => {
      setSelected && setSelected(e.target.dataset.key);
    };

    const onChange = (checked) => {
      setSelected && setSelected(checked ? "all" : "none");
    };

    const menuItems = (
      <>
        {checkboxMenuItems.map((key) => {
          const label = getRoomCheckboxTitle(t, key);

          return (
            <DropDownItem
              key={key}
              label={label}
              data-key={key}
              onClick={onSelected}
            />
          );
        })}
      </>
    );

    const headerMenu = getHeaderMenu(t);

    return (
      <TableGroupMenu
        checkboxOptions={menuItems}
        onChange={onChange}
        isChecked={isHeaderChecked}
        isIndeterminate={isHeaderIndeterminate}
        headerMenu={headerMenu}
        isInfoPanelVisible={isInfoPanelVisible}
        toggleInfoPanel={toggleInfoPanel}
      />
    );
  }, [
    t,
    isHeaderChecked,
    isHeaderIndeterminate,
    checkboxMenuItems,
    getRoomCheckboxTitle,
    getHeaderMenu,
    setSelected,
    toggleInfoPanel,
    isInfoPanelVisible,
  ]);

  return (
    <Consumer>
      {(context) => (
        <StyledContainer width={context.sectionWidth} viewAs={viewAs}>
          {isHeaderVisible ? (
            renderGroupMenu()
          ) : (
            <div className="header-container">
              <Navigation
                sectionWidth={context.sectionWidth}
                showText={showText}
                isRootFolder={true}
                canCreate={!isArchive}
                title={title}
                isDesktop={isDesktop}
                isTabletView={isTabletView}
                personal={false}
                tReady={tReady}
                navigationItems={navigationPath}
                getContextOptionsPlus={getContextOptionsPlus}
                getContextOptionsFolder={getContextOptionsFolder}
                // toggleInfoPanel={toggleInfoPanel} TODO: return after adding info-panel for rooms
                isInfoPanelVisible={isInfoPanelVisible}
              />
            </div>
          )}
        </StyledContainer>
      )}
    </Consumer>
  );
};

export default inject(
  ({
    auth,
    roomsStore,
    roomsActionsStore,
    filesStore,
    selectedFolderStore,
  }) => {
    const {
      isHeaderVisible,
      isHeaderChecked,
      isHeaderIndeterminate,
      checkboxMenuItems,
      getRoomCheckboxTitle,
      setSelected,
    } = roomsStore;

    const { getHeaderMenu } = roomsActionsStore;

    const { viewAs } = filesStore;

    const { toggleIsVisible, isVisible } = auth.infoPanelStore;

    const { title, rootFolderType, navigationPath } = selectedFolderStore;

    const isArchive = rootFolderType === FolderType.Archive;

    return {
      showText: auth.settingsStore.showText,
      isDesktop: auth.settingsStore.isDesktopClient,
      isTabletView: auth.settingsStore.isTabletView,
      viewAs,
      title,
      isArchive,
      navigationPath,

      isHeaderVisible,
      isHeaderChecked,
      isHeaderIndeterminate,
      checkboxMenuItems,
      getRoomCheckboxTitle,
      setSelected,

      getHeaderMenu,

      toggleInfoPanel: toggleIsVisible,
      isInfoPanelVisible: isVisible,
    };
  }
)(
  withTranslation([])(
    withLoader(observer(SectionHeaderContent))(<Loaders.SectionHeader />)
  )
);
