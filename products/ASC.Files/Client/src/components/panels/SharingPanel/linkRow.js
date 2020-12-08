import React from "react";
import { Row, LinkWithDropdown, ToggleButton, Icons } from "asc-web-components";
import { StyledLinkRow } from "../StyledPanels";
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
      embeddedComponentRender,
      externalAccessOptions,
      item,
      withToggle,
    } = this.props;

    const isChecked = item.rights.accessNumber !== ShareAccessRights.DenyAccess;
    const isDisabled = withToggle ? !isChecked : false;

    return (
      <StyledLinkRow withToggle={withToggle} isDisabled={isDisabled}>
        <Row
          className="link-row"
          key={`${linkText}-key_${index}`}
          element={
            withToggle ? (
              embeddedComponentRender(externalAccessOptions, item, isDisabled)
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
              color="#333333"
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
