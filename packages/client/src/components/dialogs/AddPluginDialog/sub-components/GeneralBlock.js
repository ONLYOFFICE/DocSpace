import React from "react";

import Block from "./Block";
import ControlGroup from "./ControlsGroup";
import TextInput from "@docspace/components/text-input";
import Textarea from "@docspace/components/textarea";
import FileInput from "@docspace/components/file-input";

const GeneralBlock = ({
  startValue,
  onChangeName,
  onChangeVersion,
  onChangeAuthor,
  onChangeDescription,
  onChangeImage,
  onChangePlugin,
}) => {
  return (
    <Block headerText={"General info"} headerTitle={"General info"}>
      <ControlGroup labelText={"Name"}>
        <TextInput
          value={startValue.name}
          scale={true}
          placeholder={"Enter plugin name"}
          onChange={onChangeName}
        />
      </ControlGroup>
      <ControlGroup labelText={"Version"}>
        <TextInput
          value={startValue.version}
          scale={true}
          placeholder={"Enter plugin version"}
          onChange={onChangeVersion}
        />
      </ControlGroup>
      <ControlGroup labelText={"Author"}>
        <TextInput
          value={startValue.author}
          scale={true}
          placeholder={"Enter plugin author"}
          onChange={onChangeAuthor}
        />
      </ControlGroup>
      <ControlGroup labelText={"Description"}>
        <Textarea
          value={startValue.description}
          scale={true}
          placeholder={"Enter plugin description"}
          onChange={onChangeDescription}
        />
      </ControlGroup>
      <ControlGroup labelText={"Image"}>
        <FileInput
          scale={true}
          placeholder={"Select plugin image"}
          accept={".jpg"}
          onInput={onChangeImage}
          type="file"
        />
      </ControlGroup>
      <ControlGroup labelText={"Plugin"}>
        <FileInput
          scale={true}
          placeholder={"Select plugin"}
          accept={".js"}
          onInput={onChangePlugin}
          type="file"
        />
      </ControlGroup>
    </Block>
  );
};

export default GeneralBlock;
