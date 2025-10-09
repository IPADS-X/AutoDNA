'use client';

import isEqual from 'fast-deep-equal';
import React, { memo, useCallback, useMemo, useRef } from 'react';
import { useTranslation } from 'react-i18next';
import { Virtuoso, VirtuosoHandle } from 'react-virtuoso';

import { useChatStore } from '@/store/chat';
import { topicSelectors } from '@/store/chat/selectors';
import { ChatTopic } from '@/types/topic';

import TopicItem from '../TopicItem';
import Chat from '@/locales/default/chat';

import { useState, useEffect } from 'react';

import {get, get_all_steps, global_var, state_map, eventEmitter } from '@/custom/routing';
import { SandpackConsole } from '@codesandbox/sandpack-react';
import { glob } from 'fs';

const FlatMode = memo(() => {
  const { t } = useTranslation('topic');
  const virtuosoRef = useRef<VirtuosoHandle>(null);
  // const [activeTopicId] = useChatStore((s) => [s.activeTopicId]);
  const [globalVar, setGlobalVar] = useState(global_var);
  const [, forceUpdate] = useState(0); 

  useEffect(() => {
    // 当count变化时，这个函数会被执行
    

    const handleStateChange = (data: any) => {
      console.log('global_var', globalVar);
      console.log('data', data);
      setGlobalVar(data);
      forceUpdate((s) => s + 1);
      console.log('global_var2', globalVar);
    };

    eventEmitter.subscribe('stateChange', handleStateChange);
    
    // 注意：如果useEffect中有设置定时器或其他需要清理的副作用，
    // 应该返回一个清理函数。在这个例子中，因为我们没有这类操作，所以不需要返回。
  }, [globalVar]);
  const activeTopicList = useChatStore(topicSelectors.displayTopics, isEqual);

  let topics = get_all_steps().map((step) => {
    return { favorite: false, id: step, title: step } as ChatTopic;
  });

  // let topics = [{ favorite: false, id: 'step', title: 'step' } as ChatTopic]

  let itemContent = useCallback(
    (index: number, { id, favorite, title }: ChatTopic) =>{
      console.log('id', id);
return <TopicItem active={index === globalVar.index} fav={favorite} id={id} key={id} title={title} />
// return <TopicItem active={id === state_map[globalVar.state][1][globalVar.index]} fav={favorite} id={id} key={id} title={title} />
    },
      // <TopicItem active={true} fav={favorite} title='aaaa' />,
      
      // <TopicItem active={activeTopicId === id} fav={favorite} id={id} key={id} title='aaa' />,
      // index === 0 ? (
      //   <TopicItem active={!activeTopicId} fav={favorite} title='aaaa' />
      // ) : (
      //   <TopicItem active={activeTopicId === id} fav={favorite} id={id} key={id} title='aaa' />
      // ),
    [globalVar],
  );

  const activeIndex = topics.findIndex((topic) => topic.id === state_map[globalVar.state][1][globalVar.index]);

  if (globalVar.started) {
  return (
    <Virtuoso
      // components={{ ScrollSeekPlaceholder: Placeholder }}
      computeItemKey={(_, item) => item.id}
      data={topics}
      defaultItemHeight={44}
      initialTopMostItemIndex={Math.max(activeIndex, 0)}
      itemContent={itemContent}
      overscan={44 * 10}
      ref={virtuosoRef}
      // scrollSeekConfiguration={{
      //   enter: (velocity) => Math.abs(velocity) > 350,
      //   exit: (velocity) => Math.abs(velocity) < 10,
      // }}
    />

  );}else{
    return <div></div>
  }
});

FlatMode.displayName = 'FlatMode';

export default FlatMode;
