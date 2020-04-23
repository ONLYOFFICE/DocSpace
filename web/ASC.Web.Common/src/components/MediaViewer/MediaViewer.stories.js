import React from 'react';
import { storiesOf } from '@storybook/react';
import { action } from '@storybook/addon-actions';
import Section from '../../../.storybook/decorators/section';
import MediaViewer from '.';
import withReadme from 'storybook-readme/with-readme';
import { withKnobs, boolean, text } from '@storybook/addon-knobs/react';
import Readme from './README.md';

storiesOf('Components|MediaViewer', module)
    .addDecorator(withKnobs)
    .addDecorator(withReadme(Readme))
    .add('base', () => (
        <Section>
            <MediaViewer 
                allowConvert = {true}
                playlist = {
                    [
                        {
                            id: 0,
                            src: "",
                            title: ""
                        }
                    ]
                }
                extsMediaPreviewed ={[".aac", ".flac", ".m4a", ".mp3", ".oga", ".ogg", ".wav", ".f4v", ".m4v", ".mov", ".mp4", ".ogv", ".webm"]}
                extsImagePreviewed ={[".bmp", ".gif", ".jpeg", ".jpg", ".png", ".ico", ".tif", ".tiff", ".webp"]}/>
        </Section>
    ));
