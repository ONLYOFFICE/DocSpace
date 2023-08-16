import React from "react";
import styled, { css } from "styled-components";
import { withRouter } from "react-router";
import { isMobile } from "react-device-detect";

import Row from "@docspace/components/row";
import { Base } from "@docspace/components/themes";

import withContent from "SRC_DIR/HOCs/withPeopleContent";

import UserContent from "./userContent";

const marginStyles = css`
  margin-left: -24px;
  margin-right: -24px;
  padding-left: 24px;
  padding-right: 24px;

  ${isMobile &&
  css`
    margin-left: -20px;
    margin-right: -20px;
    padding-left: 20px;
    padding-right: 20px;
  `}

  @media (max-width: 1024px) {
    margin-left: -16px;
    margin-right: -16px;
    padding-left: 16px;
    padding-right: 16px;
  }

  @media (max-width: 375px) {
    margin-left: -16px;
    margin-right: -8px;
    padding-left: 16px;
    padding-right: 8px;
  }
`;

const checkedStyle = css`
  background: ${(props) => props.theme.filesSection.rowView.checkedBackground};
  ${marginStyles}
`;

const StyledWrapper = styled.div`
  .user-item {
    border: 1px solid transparent;
    border-left: none;
    border-right: none;
    margin-left: 0;
    height: 100%;
    user-select: none;

    position: relative;
    outline: none;
    background: none !important;
  }
`;

StyledWrapper.defaultProps = { theme: Base };

const StyledSimpleUserRow = styled(Row)`
  ${(props) => (props.checked || props.isActive) && checkedStyle};

  ${!isMobile &&
  css`
    :hover {
      cursor: pointer;
      ${checkedStyle}

      margin-top: -3px;
      padding-bottom: 1px;
      border-top: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
      border-bottom: ${(props) =>
        `1px ${props.theme.filesSection.tableView.row.borderColor} solid`};
    }
  `}

  position: unset;
  margin-top: -2px;
  -webkit-tap-highlight-color: rgba(0, 0, 0, 0);

  .styled-element {
    height: 32px;
    margin-right: 12px;
  }
`;

const SimpleUserRow = (props) => {
  const {
    item,
    sectionWidth,
    contextOptionsProps,
    checkedProps,
    onContentRowSelect,
    onContentRowClick,
    element,
    //setBufferSelection,
    isActive,
    //isSeveralSelection,
    value,
  } = props;

  const isChecked = checkedProps.checked;

  const onRowClick = React.useCallback(() => {
    onContentRowClick && onContentRowClick(!isChecked, item);
  }, [isChecked, item, onContentRowClick]);

  const onRowContextClick = React.useCallback(() => {
    onContentRowClick && onContentRowClick(!isChecked, item, false);
  }, [isChecked, item, onContentRowClick]);

  return (
    <StyledWrapper
      className={`user-item row-wrapper ${
        isChecked || isActive ? "row-selected" : ""
      }`}
      value={value}
    >
      <div className="user-item">
        <StyledSimpleUserRow
          key={item.id}
          data={item}
          element={element}
          onSelect={onContentRowSelect}
          checked={isChecked}
          isActive={isActive}
          {...contextOptionsProps}
          sectionWidth={sectionWidth}
          mode={"modern"}
          className={"user-row"}
          onRowClick={onRowClick}
          rowContextClick={onRowContextClick}
        >
          <UserContent {...props} />
        </StyledSimpleUserRow>
      </div>
    </StyledWrapper>
  );
};

export default withRouter(withContent(SimpleUserRow));
