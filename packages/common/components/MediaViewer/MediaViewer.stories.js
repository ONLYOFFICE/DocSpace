import React from "react";
import { storiesOf } from "@storybook/react";
import { action } from "@storybook/addon-actions";
import Section from "../../../.storybook/decorators/section";
import MediaViewer from ".";
import Button from "@docspace/components/button";
import withReadme from "storybook-readme/with-readme";
import { withKnobs, boolean, text } from "@storybook/addon-knobs/react";
import Readme from "./README.md";

var playlist = [
  {
    id: 0,
    fileId: 0,
    src: "",
    title: "",
  },
];
class MediaViewerStory extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      visible: false,
    };
  }
  buttonClick = () => {
    this.setState({
      visible: true,
    });
  };
  onClose = () => {
    this.setState({
      visible: false,
    });
  };

  render() {
    return (
      <Section>
        <div style={{ marginBottom: "20px" }}>
          <Button label="Open" onClick={this.buttonClick} />
        </div>
        <MediaViewer
          currentFileId={0}
          allowConvert={true}
          canDelete={(fileId) => {
            if (fileId == 1) return false;
            return true;
          }}
          visible={this.state.visible}
          playlist={playlist}
          onDelete={(fileId) => action("On delete")(fileId)}
          onDownload={(fileId) => action("On download")(fileId)}
          onClose={this.onClose}
          extsMediaPreviewed={[
            ".aac",
            ".flac",
            ".m4a",
            ".mp3",
            ".oga",
            ".ogg",
            ".wav",
            ".f4v",
            ".m4v",
            ".mov",
            ".mp4",
            ".ogv",
            ".webm",
            ".avi",
            ".mpg",
            ".mpeg",
            ".wmv",
          ]}
          extsImagePreviewed={[
            ".bmp",
            ".gif",
            ".jpeg",
            ".jpg",
            ".png",
            ".ico",
            ".tif",
            ".tiff",
            ".webp",
          ]}
        />
      </Section>
    );
  }
}

storiesOf("Components|MediaViewer", module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add("base", () => (
    <Section>
      <MediaViewerStory />
    </Section>
  ));
