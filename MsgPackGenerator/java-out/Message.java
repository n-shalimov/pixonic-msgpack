package com.pixonic.battlemech.message;

import javax.xml.bind.annotation.XmlRootElement;

@XmlRootElement
public class Message extends Message {

    private java.util.Map<Integer,java.util.Date> times;
    private java.util.List<com.pixonic.battlemech.message.InnerMessage> innerMessages;
    private float floatValue;
    private Float nullableFloatValue;
    private java.util.List<Character> bytes;
    private java.util.List<String> itemList;
    private java.util.List<String> secondItemList;

    public java.util.Map<Integer,java.util.Date> getTimes() {
        return times;
    }

    public void setTimes(java.util.Map<Integer,java.util.Date> times) {
        this.times = times;
    }

    public java.util.List<com.pixonic.battlemech.message.InnerMessage> getInnerMessages() {
        return innerMessages;
    }

    public void setInnerMessages(java.util.List<com.pixonic.battlemech.message.InnerMessage> innerMessages) {
        this.innerMessages = innerMessages;
    }

    public float getFloatValue() {
        return floatValue;
    }

    public void setFloatValue(float floatValue) {
        this.floatValue = floatValue;
    }

    public Float getNullableFloatValue() {
        return nullableFloatValue;
    }

    public void setNullableFloatValue(Float nullableFloatValue) {
        this.nullableFloatValue = nullableFloatValue;
    }

    public java.util.List<Character> getBytes() {
        return bytes;
    }

    public void setBytes(java.util.List<Character> bytes) {
        this.bytes = bytes;
    }

    public java.util.List<String> getItemList() {
        return itemList;
    }

    public void setItemList(java.util.List<String> itemList) {
        this.itemList = itemList;
    }

    public java.util.List<String> getSecondItemList() {
        return secondItemList;
    }

    public void setSecondItemList(java.util.List<String> secondItemList) {
        this.secondItemList = secondItemList;
    }

}
