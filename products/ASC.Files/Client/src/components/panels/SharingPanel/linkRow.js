import React from "react";
import { Row, LinkWithDropdown, ToggleButton, Icons } from "asc-web-components";
import { StyledLinkRow } from "../StyledPanels";
import AccessComboBox from "./AccessComboBox";
import { constants } from "asc-web-common";

const { ShareAccessRights } = constants;

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
      accessOptions,
      onChangeItemAccess,
    } = this.props;

    const isChecked = item.access !== ShareAccessRights.DenyAccess;
    const isDisabled = withToggle ? !isChecked : false;

    return (
      <StyledLinkRow withToggle={withToggle} isDisabled={isDisabled}>
        <Row
          className="link-row"
          key={`${linkText}-key_${index}`}
          element={
            withToggle ? (
              <AccessComboBox
                access={item.access}
                directionX="left"
                accessOptions={accessOptions}
                onAccessChange={onChangeItemAccess}
                itemId={item.sharedTo.id}
                isDisabled={isDisabled}
              />
            ) : (
              <Icons.AccessEditIcon
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
              {t(linkText)}
            </LinkWithDropdown>
            {withToggle && (
              <div>
                <ToggleButton
                  isChecked={isChecked}
                  onChange={this.onToggleButtonChange}
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
