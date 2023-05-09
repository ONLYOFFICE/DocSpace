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
import { isMobile, mobile, isMobileOnly } from "react-device-detect";
import DropDownItem from "@docspace/components/drop-down-item";

const HeaderContainer = styled.div`
  position: relative;
  display: flex;
  align-items: center;
  width: 100vw;
  min-height: 69px;
  height: 69px;

  .arrow-button {
    margin-right: 18.5px;

    @media ${tablet} {
      padding: 8px 0 8px 8px;
      margin-left: -8px;
    }
  }

  .headline {
    font-size: 18px;
    margin-right: 16px;
  }

  .table-container_group-menu {
    margin: 0 0 0 -20px;
    -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

    width: 100%;
    height: 69px;

    /* @media ${tablet} {
      height: 60px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    }

    ${isMobile &&
    css`
      height: 60px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    `}

    @media ${mobile} {
      height: 52px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    }

    ${isMobileOnly &&
    css`
      height: 52px;
      margin: 0 0 0 -16px;
      width: calc(100% + 32px);
    `} */
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

  const headerMenu = [
    {
      id: "retry-event-option",
      label: "Retry",
      onClick: () => {
        retryWebhookEvents(checkedEventIds);
      },
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

  useEffect(() => {
    return emptyCheckedIds;
  }, []);

  return (
    <HeaderContainer>
      {isHeaderVisible ? (
        <TableGroupMenu
          checkboxOptions={menuItems}
          onChange={handleGroupSelection}
          isIndeterminate={true}
          headerMenu={headerMenu}
          withoutInfoPanelToggler
        />
      ) : (
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
      )}
    </HeaderContainer>
  );
};

export default inject(({ webhooksStore }) => {
  const { isHeaderVisible, checkAllIds, emptyCheckedIds, checkedEventIds, retryWebhookEvents } =
    webhooksStore;

  return { isHeaderVisible, checkAllIds, emptyCheckedIds, checkedEventIds, retryWebhookEvents };
})(observer(HistoryHeader));
