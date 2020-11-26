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
      type,
    } = this.props;

    const { isChecked } = this.state;

    return (
      <StyledLinkRow type={type}>
        <Row
          className="link-row"
          key={`${linkText}-key_${index}`}
          element={embeddedComponentRender(accessOptions, item)}
          contextButtonSpacerWidth="0px"
        >
          <>
            <LinkWithDropdown
              className="sharing_panel-link"
              color="black"
              dropdownType="alwaysDashed"
              data={data}
              fontSize="14px"
              fontWeight={600}
            >
              {t(linkText)}
            </LinkWithDropdown>
            {type !== "internal" && (
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
