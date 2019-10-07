import React from "react";
import { storiesOf } from "@storybook/react";
import { Container, Row, Col } from "reactstrap";
import Tooltip from "./";

const BodyStyle = { margin: 25 };

storiesOf("Components|Tooltip", module)
  // To set a default viewport for all the stories for this component
  .addParameters({ viewport: { defaultViewport: "responsive" } })
  .addParameters({ options: { showAddonPanel: false } })
  .add("all", () => (
    <Container style={{ margin: 50 }}>
      <Row style={BodyStyle}>
        <Col>
          <a
            data-for="tooltipContent"
            data-tip={"Hello_1"}
            data-event="click focus"
          >
            (❂‿❂) Click on me
          </a>
        </Col>
      </Row>
      <Row style={BodyStyle}>
        <Col>
          <a
            data-for="tooltipContent"
            data-tip={"Hello_2"}
            //data-event="click focus"
          >
            (❂‿❂) Hover on me
          </a>
        </Col>
      </Row>
      <Tooltip tooltipContent={"light"} effect={"float"} place={"top"} />
    </Container>
  ));
