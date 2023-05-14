import React, { useEffect } from "react";
import styled, { css } from "styled-components";
import { useNavigate } from "react-router-dom";
import { inject, observer } from "mobx-react";

import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";
import RetryIcon from "PUBLIC_DIR/images/refresh.react.svg?url";

import Headline from "@docspace/common/components/Headline";
import IconButton from "@docspace/components/icon-button";
import { Hint } from "../../styled-components";

import { tablet } from "@docspace/components/utils/device";

import TableGroupMenu from "@docspace/components/table-container/TableGroupMenu";
import { isMobile, isMobileOnly } from "react-device-detect";
import DropDownItem from "@docspace/components/drop-down-item";

import toastr from "@docspace/components/toast/toastr";

const HeaderContainer = styled.div`
  position: sticky;
  top: 0;
  background-color: ${(props) => props.theme.backgroundColor};
  z-index: 200;
  display: flex;
  align-items: center;
  width: 100%;
  min-height: 70px;
  flex-wrap: wrap;

  ${() =>
    isMobile &&
    css`
      margin-bottom: 11px;
    `}

  .arrow-button {
    margin-right: 18.5px;

    @media ${tablet} {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
      margin-right: 18.5px;
    }
  }

  .headline {
    font-size: 18px;
    margin-right: 16px;
  }

  .table-container_group-menu {
    margin: 0 0 0 -20px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    flex: 0 0 auto;

    width: calc(100% + 45px);
    height: 69px;

    ${() =>
      isMobile &&
      css`
        height: 60px;
        margin: 0 0 0 -16px;
        width: calc(100% + 32px);
      `}
    ${() =>
      isMobileOnly &&
      css`
        height: 48px;
        margin: -49px 0 0 -17px;
        width: calc(100% + 32px);
      `}
  }
`;

const HistoryHeader = (props) => {
  const {
    isHeaderVisible,
    checkedEventIds,
    checkAllIds,
    emptyCheckedIds,
    historyWebhooks,
    retryWebhookEvents,
  } = props;
  const navigate = useNavigate();
  const onBack = () => {
    navigate(-1);
  };

  const handleGroupSelection = (isChecked) => {
    isChecked ? checkAllIds(historyWebhooks) : emptyCheckedIds();
  };

  const handleRetryAll = async () => {
    await retryWebhookEvents(checkedEventIds);
    toastr.success(`Webhooks redelivered: ${checkedEventIds.length}`, <b>Done</b>);
  };

  const headerMenu = [
    {
      id: "retry-event-option",
      label: "Retry",
      onClick: handleRetryAll,
      iconUrl: RetryIcon,
    },
  ];

  const menuItems = (
    <>
      <DropDownItem
        key="select-all-event-ids"
        label="Select all"
        data-index={0}
        onClick={() => checkAllIds(historyWebhooks)}
      />
      <DropDownItem
        key="unselect-all-event-ids"
        label="Unselect all"
        data-index={1}
        onClick={emptyCheckedIds}
      />
    </>
  );

  const NavigationHeader = () => (
    <>
      <IconButton
        iconName={ArrowPathReactSvgUrl}
        size="17"
        isFill={true}
        onClick={onBack}
        className="arrow-button"
      />
      <Headline type="content" truncate={true} className="headline">
        History
      </Headline>
      <Hint backgroundColor="#F8F9F9" color="#555F65">
        Deliveries are automatically deleted after 15 days
      </Hint>
    </>
  );

  useEffect(() => {
    return emptyCheckedIds;
  }, []);

  return (
    <HeaderContainer>
      {isMobileOnly ? (
        <>
          {isHeaderVisible && (
            <TableGroupMenu
              checkboxOptions={menuItems}
              onChange={handleGroupSelection}
              isIndeterminate={true}
              headerMenu={headerMenu}
              withoutInfoPanelToggler
            />
          )}
          <NavigationHeader />
        </>
      ) : isHeaderVisible ? (
        <TableGroupMenu
          checkboxOptions={menuItems}
          onChange={handleGroupSelection}
          isIndeterminate={true}
          headerMenu={headerMenu}
          withoutInfoPanelToggler
        />
      ) : (
        <NavigationHeader />
      )}
    </HeaderContainer>
  );
};

export default inject(({ webhooksStore }) => {
  const { isHeaderVisible, checkAllIds, emptyCheckedIds, checkedEventIds, retryWebhookEvents } =
    webhooksStore;

  return { isHeaderVisible, checkAllIds, emptyCheckedIds, checkedEventIds, retryWebhookEvents };
})(observer(HistoryHeader));
