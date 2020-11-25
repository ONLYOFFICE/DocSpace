import React from "react";
import { Row, LinkWithDropdown, Icons } from "asc-web-components";

class LinkRow extends React.Component {
  constructor(props) {
    super(props);

    this.state = {};
  }

  render() {
    const { linkText, data, index, t } = this.props;

    return (
      <Row
        key={`${linkText}-key_${index}`}
        //element={embeddedComponentRender(accessOptions, item)}
        element={
          <Icons.AccessEditIcon
            size="medium"
            className="sharing_panel-owner-icon"
          />
        }
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
        </>
      </Row>
    );
  }
}

export default LinkRow;
