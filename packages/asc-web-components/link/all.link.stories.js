import React from "react";
import { storiesOf } from "@storybook/react";
import Link from ".";

const rowStyle = { marginTop: 8, fontSize: 12 };

const headerStyle = {
  padding: "8px 0 0 40px",
  fontSize: 16,
};

storiesOf("Components|Link", module)
  .addParameters({ options: { showAddonPanel: false } })
  .add("all", () => (
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
  ));
