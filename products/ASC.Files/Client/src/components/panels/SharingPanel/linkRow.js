import React from "react";
import { Row, LinkWithDropdown, ToggleButton } from "asc-web-components";
import { StyledLinkRow } from "../StyledPanels";

class LinkRow extends React.Component {
  constructor(props) {
    super(props);

    this.state = {
      isChecked: false,
    };
  }

  onChangeToggle = (e) => {
    const isChecked = this.props.onChangeToggle(e);
    this.setState({
      isChecked: isChecked,
    });
  };

  render() {
    const {
      linkText,
      data,
      index,
      t,
      embeddedComponentRender,
      accessOptions,
      item,
      withToggle,
    } = this.props;

    const { isChecked } = this.state;
    const isDisabled = withToggle ? !isChecked : false;

    return (
      <StyledLinkRow withToggle={withToggle} isDisabled={isDisabled}>
        <Row
          className="link-row"
          key={`${linkText}-key_${index}`}
          element={embeddedComponentRender(accessOptions, item, isDisabled)}
          contextButtonSpacerWidth="0px"
        >
          <>
            <LinkWithDropdown
              className="sharing_panel-link"
              color="#333333"
              dropdownType="alwaysDashed"
              data={data}
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
                  onChange={this.onChangeToggle}
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
