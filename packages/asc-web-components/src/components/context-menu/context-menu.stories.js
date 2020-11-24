import React from "react";
import { storiesOf } from "@storybook/react";
import RowContainer from "../row-container";
import RowContent from "../row-content";
import Row from "../row";
import Section from "../../../.storybook/decorators/section";
import Readme from "./README.md";
import withReadme from "storybook-readme/with-readme";

const getRndString = (n) =>
  Math.random()
    .toString(36)
    .substring(2, n + 2);

storiesOf("Components|ContextMenu", module)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    const array = Array.from(Array(10).keys());

    return (
      <Section>
        <RowContainer manualHeight="300px">
          {array.map((item, index) => {
            return (
              <Row
                key={`${item + 1}`}
                contextOptions={
                  index !== 3
                    ? [
                        { key: 1, label: getRndString(5) },
                        { key: 2, label: getRndString(5) },
                        { key: 3, label: getRndString(5) },
                        { key: 4, label: getRndString(5) },
                      ]
                    : []
                }
              >
                <RowContent>
                  <span>{getRndString(5)}</span>
                  <></>
                </RowContent>
              </Row>
            );
          })}
        </RowContainer>
      </Section>
    );
  });
