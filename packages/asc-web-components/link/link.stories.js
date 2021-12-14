import React from "react";
import Link from "./";

export default {
  title: "Components/Link",
  component: Link,
  parameters: {
    docs: {
      description: {
        component: `It is a link with 2 types:

1. page - simple link which refer to other pages and parts of current page;
2. action - link, which usually hasn't hyperlink and do anything on click - open dropdown, filter data, etc`,
      },
    },
  },
  argType: {
    color: { control: "color" },
    onClick: { action: "clickActionLink" },
  },
};

const Template = ({ label, onClick, href, ...args }) => {
  const actionProps = href && href.length > 0 ? { href } : { onClick };
  return (
    <Link {...args} {...actionProps}>
      {label}
    </Link>
  );
};

export const Default = Template.bind({});
Default.args = {
  href: "http://github.com",
  label: "Simple label",
  type: "page",
  fontSize: "13px",
  fontWeight: "400",
  isBold: false,
  target: "_blank",
  isHovered: false,
  noHover: false,
  isSemitransparent: false,
  isTextOverflow: false,
};

const AllTemplate = (args) => {
  const rowStyle = { marginTop: 8, fontSize: 12 };

  const headerStyle = {
    padding: "8px 0 0 40px",
    fontSize: 16,
  };
  return (
    <div style={{ display: "grid", gridTemplateColumns: "1fr 1fr" }}>
      <div style={headerStyle}>
        <div>Page links:</div>
        <div>
          <div style={rowStyle}>
            <Link type="page" href="https://github.com" isBold={true}>
              Bold black page link
            </Link>
          </div>
          <div style={rowStyle}>
            <Link type="page" href="https://github.com">
              Black page link
            </Link>
          </div>
          <div style={rowStyle}>
            <Link type="page" href="https://github.com" isHovered={true}>
              Black hovered page link
            </Link>
          </div>
          <div style={rowStyle}>
            <Link
              type="page"
              href="https://github.com"
              isSemitransparent={true}
            >
              Semitransparent black page link
            </Link>
          </div>
        </div>
      </div>
      <div style={headerStyle}>
        <div>Action links:</div>
        <div style={rowStyle}>
          <Link type="action" isBold={true}>
            Bold black action link
          </Link>
        </div>
        <div style={rowStyle}>
          <Link type="action">Black action link</Link>
        </div>
        <div style={rowStyle}>
          <Link type="action" isHovered={true}>
            Black hovered action link
          </Link>
        </div>
        <div style={rowStyle}>
          <Link type="action" isSemitransparent={true}>
            Semitransparent black action link
          </Link>
        </div>
      </div>
    </div>
  );
};

export const AllLinks = AllTemplate.bind({});
