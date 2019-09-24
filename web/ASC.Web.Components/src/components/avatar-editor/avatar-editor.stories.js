import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import { withKnobs, boolean, text, select } from '@storybook/addon-knobs/react';
import withReadme from 'storybook-readme/with-readme';
import Readme from './README.md';
import AvatarEditor from '.';
import Avatar from '../avatar';
import Section from '../../../.storybook/decorators/section';

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
          onImageChange={this.onImageChange}
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