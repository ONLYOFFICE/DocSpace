import React from "react";
import {
  Row,
  LinkWithDropdown,
  ToggleButton,
  Icons,
  ComboBox,
} from "asc-web-components";
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
      advancedOptions,
      item,
      withToggle,
    } = this.props;

    //console.log("LinkRow item", item);

    const isChecked = item.access !== ShareAccessRights.DenyAccess;
    const isDisabled = withToggle ? !isChecked : false;

    return (
      <StyledLinkRow withToggle={withToggle} isDisabled={isDisabled}>
        <Row
          className="link-row"
          key={`${linkText}-key_${index}`}
          element={
            withToggle ? (
              //              embeddedComponentRender(externalAccessOptions, item, isDisabled)
              <ComboBox
                advancedOptions={advancedOptions}
                options={[]}
                selectedOption={{ key: 0 }}
                size="content"
                className="panel_combo-box"
                scaled={false}
                directionX="left"
                disableIconClick={false}
                isDisabled={isDisabled}
              >
                {React.createElement(Icons["EyeIcon"], {
                  //{React.createElement(Icons[item.rights.icon], {
                  size: "medium",
                  className: "sharing-access-combo-box-icon",
                })}
              </ComboBox>
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
