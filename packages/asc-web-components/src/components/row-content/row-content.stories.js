import React from "react";
import { storiesOf } from "@storybook/react";
import { BooleanValue } from "react-values";
import withReadme from "storybook-readme/with-readme";
import Readme from "./README.md";
import Section from "../../../.storybook/decorators/section";
import RowContent from ".";
import Link from "../link";
import { Icons } from "../icons";
import Checkbox from "../checkbox";

storiesOf("Components|RowContent", module)
  .addDecorator(withReadme(Readme))
  .add("base", () => {
    return (
      <Section>
        <h3>Base demo</h3>
        <div style={{ height: "16px" }}></div>
        <RowContent>
          <Link
            type="page"
            title="Demo"
            isBold={true}
            fontSize="15px"
            color="#333333"
          >
            Demo
          </Link>
          <>
            <Icons.SendClockIcon size="small" isfill={true} color="#3B72A7" />
            <Icons.CatalogSpamIcon size="small" isfill={true} color="#3B72A7" />
          </>
          <Link type="page" title="Demo" fontSize="12px" color="#A3A9AE">
            Demo
          </Link>
          <Link
            containerWidth="160px"
            type="action"
            title="Demo"
            fontSize="12px"
            color="#A3A9AE"
          >
            Demo
          </Link>
          <Link
            type="page"
            title="0 000 0000000"
            fontSize="12px"
            color="#A3A9AE"
          >
            0 000 0000000
          </Link>
          <Link
            containerWidth="160px"
            type="page"
            title="demo@demo.com"
            fontSize="12px"
            color="#A3A9AE"
          >
            demo@demo.com
          </Link>
        </RowContent>
        <RowContent>
          <Link
            type="page"
            title="Demo Demo"
            isBold={true}
            fontSize="15px"
            color="#333333"
          >
            Demo Demo
          </Link>
          <>
            <Icons.CatalogSpamIcon size="small" isfill={true} color="#3B72A7" />
          </>
          <></>
          <Link
            containerWidth="160px"
            type="action"
            title="Demo Demo"
            fontSize="12px"
            color="#A3A9AE"
          >
            Demo Demo
          </Link>
          <Link
            type="page"
            title="0 000 0000000"
            fontSize="12px"
            color="#A3A9AE"
          >
            0 000 0000000
          </Link>
          <Link
            containerWidth="160px"
            type="page"
            title="demo.demo@demo.com"
            fontSize="12px"
            color="#A3A9AE"
          >
            demo.demo@demo.com
          </Link>
        </RowContent>
        <RowContent>
          <Link
            type="page"
            title="Demo Demo Demo"
            isBold={true}
            fontSize="15px"
            color="#333333"
          >
            Demo Demo Demo
          </Link>
          <></>
          <></>
          <Link
            containerWidth="160px"
            type="action"
            title="Demo Demo Demo"
            fontSize="12px"
            color="#A3A9AE"
          >
            Demo Demo Demo
          </Link>
          <Link
            type="page"
            title="0 000 0000000"
            fontSize="12px"
            color="#A3A9AE"
          >
            0 000 0000000
          </Link>
          <Link
            containerWidth="160px"
            type="page"
            title="demo.demo.demo@demo.com"
            fontSize="12px"
            color="#A3A9AE"
          >
            demo.demo.demo@demo.com
          </Link>
        </RowContent>
        <RowContent>
          <Link
            type="page"
            title="Demo Demo Demo Demo"
            isBold={true}
            fontSize="15px"
            color="#333333"
          >
            Demo Demo Demo Demo
          </Link>
          <>
            <Icons.SendClockIcon size="small" isfill={true} color="#3B72A7" />
          </>
          <Link type="page" title="Demo" fontSize="12px" color="#A3A9AE">
            Demo
          </Link>
          <Link
            containerWidth="160px"
            type="action"
            title="Demo Demo Demo Demo"
            fontSize="12px"
            color="#A3A9AE"
          >
            Demo Demo Demo Demo
          </Link>
          <Link
            type="page"
            title="0 000 0000000"
            fontSize="12px"
            color="#A3A9AE"
          >
            0 000 0000000
          </Link>
          <Link
            containerWidth="160px"
            type="page"
            title="demo.demo.demo.demo@demo.com"
            fontSize="12px"
            color="#A3A9AE"
          >
            demo.demo.demo.demo@demo.com
          </Link>
        </RowContent>
        <div style={{ height: "36px" }}></div>
        <h3>Custom elements</h3>
        <div style={{ height: "16px" }}></div>
        <RowContent disableSideInfo={true}>
          <Link
            type="page"
            title="John Doe"
            isBold={true}
            fontSize="15px"
            color="#333333"
          >
            John Doe
          </Link>
          <></>
          <BooleanValue>
            {({ value, toggle }) => (
              <Checkbox
                id="1"
                name="sample"
                isChecked={value}
                onChange={(e) => {
                  toggle(e.target.checked);
                }}
              />
            )}
          </BooleanValue>
          <BooleanValue>
            {({ value, toggle }) => (
              <Checkbox
                id="2"
                name="sample"
                isChecked={value}
                onChange={(e) => {
                  toggle(e.target.checked);
                }}
              />
            )}
          </BooleanValue>
          <BooleanValue>
            {({ value, toggle }) => (
              <Checkbox
                id="3"
                name="sample"
                isChecked={value}
                onChange={(e) => {
                  toggle(e.target.checked);
                }}
              />
            )}
          </BooleanValue>
          <BooleanValue>
            {({ value, toggle }) => (
              <Checkbox
                id="4"
                name="sample"
                isChecked={value}
                onChange={(e) => {
                  toggle(e.target.checked);
                }}
              />
            )}
          </BooleanValue>
          <BooleanValue>
            {({ value, toggle }) => (
              <Checkbox
                id="5"
                name="sample"
                isChecked={value}
                onChange={(e) => {
                  toggle(e.target.checked);
                }}
              />
            )}
          </BooleanValue>
          <BooleanValue>
            {({ value, toggle }) => (
              <Checkbox
                id="6"
                name="sample"
                isChecked={value}
                onChange={(e) => {
                  toggle(e.target.checked);
                }}
              />
            )}
          </BooleanValue>
          <BooleanValue>
            {({ value, toggle }) => (
              <Checkbox
                id="7"
                name="sample"
                isChecked={value}
                onChange={(e) => {
                  toggle(e.target.checked);
                }}
              />
            )}
          </BooleanValue>
        </RowContent>
      </Section>
    );
  });
