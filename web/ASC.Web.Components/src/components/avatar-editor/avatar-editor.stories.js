import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, text, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import AvatarEditor from '.';
import Avatar from '../avatar';
import Section from '../../../.storybook/decorators/section';

const displayType = ['auto', 'modal', 'aside'];
class AvatarEditorStory extends React.Component  {
  constructor(props) {
    super(props);

    this.state = {
      isOpen: false,
      userImage: null
    }

    this.openEditor = this.openEditor.bind(this);
    this.onClose = this.onClose.bind(this);
    this.onSave = this.onSave.bind(this);
    this.onLoadFile = this.onLoadFile.bind(this)
    this.onImageChange = this.onImageChange.bind(this)
    this.onDeleteImage = this.onDeleteImage.bind(this)
    this.onLoadFile = this.onLoadFile.bind(this)
    
  }
  onDeleteImage(){
    action('onDeleteImage');
  }
  onImageChange(img){
    action('onLoadFile');
    this.setState({
      userImage: img
    })
  }
  onLoadFile(file){
    action('onLoadFile')(file);
  }
  onSave(isUpdate, data){
    action('onSave')(isUpdate, data);
    this.setState({
      isOpen: false
    })
  }
  openEditor(){
    this.setState({
      isOpen: true
    })
  }
  onClose(){
    action('onClose');
    this.setState({
      isOpen: false
    })
  }
  render(){
    return(
      <div>
        <Avatar
            size='max'
            role='user'
            source={this.state.userImage }
            editing={true}
            editAction={this.openEditor}
        />
        <AvatarEditor
          visible={this.state.isOpen}
          onClose={this.onClose}
          onSave={this.onSave}
          onDeleteImage={this.onDeleteImage}
          onImageChange={this.onImageChange}
          onLoadFile={this.onLoadFile}
          headerLabel      ={text('headerLabel', 'Edit Photo')}
          chooseFileLabel  ={text('chooseFileLabel', 'Drop files here, or click to select files')}
          chooseMobileFileLabel={text('chooseMobileFileLabel', 'Click to select files')}
          saveButtonLabel  ={text('saveButtonLabel', 'Save')}
          maxSizeFileError ={text('maxSizeFileError', 'Maximum file size exceeded')}
          unknownTypeError ={text('unknownTypeError', 'Unknown image file type')}
          unknownError     ={text('unknownError', 'Error')}
          displayType      ={select('displayType', displayType, 'auto')}
          />
      </div>
    )
  }
}

storiesOf('Components|AvatarEditor', module)
  .addDecorator(withKnobs)
  .addDecorator(withReadme(Readme))
  .add('base', () => {
   
    return (
      <Section>
        <AvatarEditorStory />
    </Section>
    );
  });