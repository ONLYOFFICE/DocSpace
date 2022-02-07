import React from "react";
import styled, { css } from "styled-components";
import Row from "@appserver/components/row";
import LinkWithDropdown from "@appserver/components/link-with-dropdown";
import ToggleButton from "@appserver/components/toggle-button";
import { StyledLinkRow } from "../StyledPanels";
import AccessComboBox from "./AccessComboBox";
import { ShareAccessRights } from "@appserver/common/constants";
import AccessEditIcon from "../../../../../../../public/images/access.edit.react.svg";
import CopyIcon from "../../../../../../../public/images/copy.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

const StyledAccessEditIcon = styled(AccessEditIcon)`
  ${commonIconsStyles}
  path {
    fill: "#A3A9AE";
  }
`;

const StyledCopyIcon = styled(CopyIcon)`
  ${commonIconsStyles}

  cursor: pointer;

  ${(props) =>
    props.isDisabled &&
    css`
      cursor: default;
      pointer-events: none;
    `}
`;

class LinkRow extends React.Component {
  onToggleButtonChange = () => {
    const { onToggleLink, item } = this.props;

    onToggleLink(item);
  };

  render() {
    const {
      linkText,
      options,
      index,
      t,
      item,
      withToggle,
      externalAccessOptions,
      onChangeItemAccess,
      isLoading,
      onCopyLink,
    } = this.props;

    const isChecked = item.access !== ShareAccessRights.DenyAccess;
    const disableLink = withToggle ? !isChecked : false;
    const isDisabled = isLoading || disableLink;

    return (
      <StyledLinkRow
        withToggle={withToggle}
        isDisabled={isDisabled}
        className="link-row__container"
      >
        <Row
          className="link-row"
          key={`${linkText.replace(" ", "-")}-key_${index}`}
          element={
            withToggle ? (
              <AccessComboBox
                t={t}
                access={item.access}
                directionX="left"
                accessOptions={externalAccessOptions}
                onAccessChange={onChangeItemAccess}
                itemId={item.sharedTo.id}
                isDisabled={isDisabled}
                disableLink={disableLink}
              />
            ) : (
              <StyledAccessEditIcon
                size="medium"
                className="sharing_panel-owner-icon"
              />
            )
          }
          contextButtonSpacerWidth="0px"
        >
          <>
            <div className="sharing_panel-link-container">
              <LinkWithDropdown
                className="sharing_panel-link"
                color="#333"
                dropdownType="alwaysDashed"
                data={options}
                fontSize="13px"
                fontWeight={600}
                isDisabled={isDisabled}
              >
                {linkText}
              </LinkWithDropdown>
              {onCopyLink && (
                <StyledCopyIcon
                  isDisabled={isDisabled}
                  size="medium"
                  onClick={onCopyLink}
                  title={t("CopyExternalLink")}
                />
              )}
            </div>
            {withToggle && (
              <div>
                <ToggleButton
                  isChecked={isChecked}
                  onChange={this.onToggleButtonChange}
                  isDisabled={isLoading}
                  className="sharing-row__toggle-button"
                />
              </div>
            )}
          </>
        </Row>
      </StyledLinkRow>
    );
  }
}

export default LinkRow;
