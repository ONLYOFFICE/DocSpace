import React from "react";
import styled from "styled-components";
import Row from "@appserver/components/row";
import LinkWithDropdown from "@appserver/components/link-with-dropdown";
import ToggleButton from "@appserver/components/toggle-button";
import { StyledLinkRow } from "../StyledPanels";
import AccessComboBox from "./AccessComboBox";
import { ShareAccessRights } from "@appserver/common/constants";
import AccessEditIcon from "../../../../../../../public/images/access.edit.react.svg";
import commonIconsStyles from "@appserver/components/utils/common-icons-style";

const StyledAccessEditIcon = styled(AccessEditIcon)`
  ${commonIconsStyles}
  path {
    fill: "#A3A9AE";
  }
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
    } = this.props;

    const isChecked = item.access !== ShareAccessRights.DenyAccess;
    const disableLink = withToggle ? !isChecked : false;
    const isDisabled = isLoading || disableLink;

    return (
      <StyledLinkRow withToggle={withToggle} isDisabled={isDisabled}>
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
            <LinkWithDropdown
              className="sharing_panel-link"
              color="#333"
              dropdownType="alwaysDashed"
              data={options}
              fontSize="14px"
              fontWeight={600}
              isDisabled={isDisabled}
            >
              {linkText}
            </LinkWithDropdown>
            {withToggle && (
              <div>
                <ToggleButton
                  isChecked={isChecked}
                  onChange={this.onToggleButtonChange}
                  isDisabled={isLoading}
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
