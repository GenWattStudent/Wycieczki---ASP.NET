class BreakInfo {
  constructor(timeline, index, date) {
    this.timeline = timeline
    this.index = index
    this.date = date
    this.timelineStartsInP = $(`${timeline.parent} #timeline-starts-in p`)
  }

  setText(date) {
    const timeRemaining = this.getTimeRemaining(date)
    this.timelineStartsInP.text(timeRemaining)

    if (date <= 0) {
      this.timelineStartsInP.text('00d 00h 00m 00s')
      const timelineStartsIn = $(`${this.timeline.parent} #timeline-starts-in`)
      timelineStartsIn.removeClass('d-flex').addClass('d-none')
      document.dispatchEvent(new CustomEvent('breakend', { detail: { parent: this.timeline.parent } }))
    }
  }

  setBreak() {
    const isCardOnTheLeftSideOfTimeLine = this.index % 2 === 0
    const timelineStartsIn = $(`${this.timeline.parent} #timeline-starts-in`)
    const timelineStartsInWidth = timelineStartsIn.width()
    const margin = 10
    const leftOffset = isCardOnTheLeftSideOfTimeLine ? `-${timelineStartsInWidth + margin}px` : `${margin}px`

    timelineStartsIn.removeClass('d-none').addClass('d-flex')
    timelineStartsIn.css({
      top: `${this.timeline.dots[this.index].offsetTop - timelineStartsIn.height() / 2}px`,
      left: leftOffset,
    })

    this.setText(this.date - new Date())
  }

  getTimeRemaining(date) {
    const days = Math.floor(date / (1000 * 60 * 60 * 24))
    const hours =
      Math.floor((date % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60)) >= 10
        ? Math.floor((date % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))
        : `0${Math.floor((date % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60))}`

    const minutes =
      Math.floor((date % (1000 * 60 * 60)) / (1000 * 60)) >= 10
        ? Math.floor((date % (1000 * 60 * 60)) / (1000 * 60))
        : `0${Math.floor((date % (1000 * 60 * 60)) / (1000 * 60))}`

    const seconds =
      Math.floor((date % (1000 * 60)) / 1000) >= 10
        ? Math.floor((date % (1000 * 60)) / 1000)
        : `0${Math.floor((date % (1000 * 60)) / 1000)}`
    console.log(`${days}d ${hours}h ${minutes}m ${seconds}s`)
    return `${days}d ${hours}h ${minutes}m ${seconds}s`
  }
}

class Timeline {
  constructor(mealsModel, parent) {
    this.parent = parent
    this.mealsModel = mealsModel.$values.sort((a, b) => new Date(a.start) - new Date(b.start))
    this.timelineItem = $(`${parent} .checkpoint`)
    this.timeLineProgress = $(`${parent} #timeline-progress`)
    this.currentTimelineProgress = $(`${parent} #current-timeline-progress`)
    this.currentTimelineProgressIndicator = $(`${parent} #current-timeline-progress-indicator`)
    this.dots = []
    this.currentMeal = null
    this.progress = 0
    this.breakInfo = null

    document.addEventListener('breakend', (e) => {
      const { parent } = e.detail
      if (parent === this.parent) {
        this.breakInfo = null
      }
    })

    window.addEventListener('resize', (e) => this.init())
  }

  init = () => {
    // remove all dots
    $(`${this.parent} .line-dot`).remove()

    this.timelineItem.each((index, element) => {
      const itemTop = element.offsetTop + element.offsetHeight / 2
      const lineDotIndicator = document.createElement('div')

      lineDotIndicator.classList.add('line-dot', 'bg-primary')
      lineDotIndicator.style.top = `${itemTop}px`

      this.timeLineProgress.append(lineDotIndicator)
    })
    this.dots = $(`${this.parent} .line-dot`)
    this.update()
    // transition end event
    this.currentTimelineProgressIndicator.on('transitionend', () => {
      $(this.parent).animate(
        {
          scrollTop: this.currentTimelineProgressIndicator.offset().top - $(this.parent).offset().top - 100,
        },
        1000
      )
    })
  }

  update() {
    const { currentMeal, index } = this.getCurrentMeal(this.mealsModel)
    this.progress = this.getTimelineProgress(currentMeal, index)
    this.updateTimelineProgress()
    if (this.breakInfo) this.breakInfo.setBreak()
  }

  clamp = (num, min, max) => Math.min(Math.max(num, min), max)

  getDatePercent(start, end) {
    const percent = this.clamp((new Date() - start) / (end - start), -1, 1)
    return percent > 1 ? 1 : percent < 0 ? 1 + percent : percent
  }

  getHeightWhenNotPrevMeal(meal, index) {
    const checkpointHeight = this.dots[index].offsetTop
    const nextMealHeight = this.dots[index + 1].offsetTop
    const percent = this.getDatePercent(new Date(meal.start), new Date(meal.end))
    return percent * (nextMealHeight - checkpointHeight) + checkpointHeight
  }

  getHeightWhenNotNextMeal(meal, index) {
    const checkpointHeight = this.dots[index].offsetTop
    const percent = this.getDatePercent(new Date(meal.start), new Date(meal.end))
    const currentMealHeight = (this.timeLineProgress.height() - checkpointHeight) * percent
    return currentMealHeight + checkpointHeight
  }

  getHeightWhenNextAndPrevMeal(meal, index) {
    const checkpointHeight = this.dots[index].offsetTop
    const percent = this.getDatePercent(new Date(meal.start), new Date(meal.end))
    const currentMealHeight = (this.dots[index + 1].offsetTop - checkpointHeight) * percent

    return currentMealHeight + checkpointHeight
  }

  getTimelineProgress(meal, index) {
    console.log(meal)
    if (meal) {
      const prevMeal = this.mealsModel[index - 1]
      const nextMeal = this.mealsModel[index + 1]
      if (!prevMeal && nextMeal) return this.getHeightWhenNotPrevMeal(meal, index)
      else if (!nextMeal) return this.getHeightWhenNotNextMeal(meal, index)
      else return this.getHeightWhenNextAndPrevMeal(meal, index)
    } else {
      // console.log(this.mealsModel)
      const prevMeal = this.getLastestMealFromNow(this.mealsModel)
      const nextMeal = this.getNextMealFromNow(this.mealsModel)

      if (prevMeal && nextMeal) {
        const nextIndex = this.mealsModel.indexOf(nextMeal)
        const checkpointHeight = this.dots[nextIndex].offsetTop
        this.breakInfo = new BreakInfo(this, nextIndex, new Date(nextMeal.start))
        this.breakInfo.setBreak()
        return checkpointHeight
      } else {
        const nextMeal = this.getNextMealFromNow(this.mealsModel)
        if (nextMeal) {
          const nextIndex = this.mealsModel.indexOf(nextMeal)
          const checkpointHeight = this.dots[nextIndex].offsetTop
          const percent = this.getDatePercent(new Date(nextMeal.start), new Date(nextMeal.end))

          return checkpointHeight * percent
        } else {
          console.log(this.timeLineProgress)
          return this.timeLineProgress.height()
        }
      }
    }
  }

  getCurrentMeal(meals) {
    let currentMeal = null
    let index = 0
    const now = new Date()

    for (let i = 0; i < meals.length; i++) {
      const mealTime = new Date(meals[i].start)
      if (mealTime < now && now < new Date(meals[i].end)) {
        currentMeal = meals[i]
        index = i
      }
    }

    return { currentMeal, index }
  }

  getLastestMealFromNow(meals) {
    const now = new Date()
    let lastestMeal = null

    for (let i = 0; i < meals.length; i++) {
      const mealTime = new Date(meals[i].start)
      if (mealTime < now) {
        lastestMeal = meals[i]
      }
    }

    return lastestMeal
  }

  getNextMealFromNow(meals) {
    const now = new Date()
    let nextMeal = null

    for (let i = 0; i < meals.length; i++) {
      const mealTime = new Date(meals[i].start)
      if (mealTime > now) {
        nextMeal = meals[i]
        break
      }
    }

    return nextMeal
  }

  updateTimelineProgress() {
    const indicatorHeight = 0
    this.currentTimelineProgress.css('height', `${this.progress}px`)
    this.currentTimelineProgressIndicator.css('top', `${this.progress - indicatorHeight}px`)
  }
}
