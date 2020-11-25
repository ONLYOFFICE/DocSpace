import React from "react";
import { Row, LinkWithDropdown, ToggleButton } from "asc-web-components";
import { StyledLinkRow } from "../StyledPanels";

class LinkRow extends React.Component {
  constructor(props) {
    super(props);

    this.state = {};
  }

  render() {
    const {
      linkText,
      data,
      index,
      t,
      embeddedComponentRender,
      accessOptions,
      item,
    } = this.props;

    return (
      <StyledLinkRow>
        <Row
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
            <div>
              <ToggleButton />
            </div>
          </>
        </Row>
      </StyledLinkRow>
    );
  }
}

export default LinkRow;
